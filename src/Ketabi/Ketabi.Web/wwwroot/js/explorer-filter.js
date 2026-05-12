/**
 * Explorer Filter - AJAX-based instant filtering and search
 */

document.addEventListener("DOMContentLoaded", () => {
  initializeFilterHandlers();
});

function initializeFilterHandlers() {
  // Handle search input with debouncing
  const searchInput = document.getElementById("explorer-search-input");
  if (searchInput) {
    searchInput.oninput = debounce(() => {
      performFilter();
    }, 300);
  }

  const searchButton = document.querySelector(".explorer-search__btn");
  if (searchButton) {
    searchButton.onclick = (e) => {
      e.preventDefault();
      performFilter();
    };
  }

  // Handle filter bar clicks (category, mode, clear)
  const filterBar = document.querySelector(".explorer-filterbar");
  if (filterBar) {
    const links = filterBar.querySelectorAll("a");
    links.forEach((link) => {
      link.onclick = (e) => {
        e.preventDefault();
        const url = new URL(link.href, window.location.origin);
        performFilter(url);
      };
    });
  }

  // Handle clear search chip (if it exists)
  const gridContainer = document.getElementById("explorer-grid-container");
  if (gridContainer) {
    const clearSearchLink = gridContainer.querySelector(
      'a[aria-label="Clear search"]',
    );
    if (clearSearchLink) {
      clearSearchLink.onclick = (e) => {
        e.preventDefault();
        const url = new URL(clearSearchLink.href, window.location.origin);
        performFilter(url);
      };
    }
  }
}

function debounce(fn, delay) {
  let timeout;
  return function (...args) {
    clearTimeout(timeout);
    timeout = setTimeout(() => fn.apply(this, args), delay);
  };
}

function performFilter(customUrl) {
  // Build filter parameters from current state
  let url;

  if (customUrl) {
    // Extract query parameters from the link URL and convert to API endpoint
    url = new URL("/api/books/filter", window.location.origin);

    // Copy query parameters from the link to the API URL
    const q = customUrl.searchParams.get("q");
    const categoryId = customUrl.searchParams.get("categoryId");
    const mode = customUrl.searchParams.get("mode");

    if (q) url.searchParams.set("q", q);
    if (categoryId) url.searchParams.set("categoryId", categoryId);
    if (mode) url.searchParams.set("mode", mode);
  } else {
    // Get current filter values
    const searchQuery =
      document.getElementById("explorer-search-input")?.value || "";
    const currentUrl = new URL(window.location.href);

    url = new URL("/api/books/filter", window.location.origin);

    if (searchQuery) {
      url.searchParams.set("q", searchQuery);
    }

    // Preserve other filters from URL
    const categoryId = currentUrl.searchParams.get("categoryId");
    const mode = currentUrl.searchParams.get("mode");

    if (categoryId) {
      url.searchParams.set("categoryId", categoryId);
    }
    if (mode) {
      url.searchParams.set("mode", mode);
    }
  }

  // Show loading state
  showLoadingState();

  // Make AJAX request
  fetch(url.toString(), {
    headers: {
      Accept: "application/json",
      "X-Requested-With": "XMLHttpRequest",
    },
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      return response.json();
    })
    .then((data) => {
      // Update filter bar
      const filterBarContainer = document.querySelector(".explorer-filterbar");
      if (filterBarContainer && data.filterBar) {
        filterBarContainer.outerHTML = data.filterBar;
        initializeFilterHandlers(); // Re-attach event listeners
      }

      // Update book grid container with the entire grid HTML
      const gridContainer = document.getElementById("explorer-grid-container");
      if (gridContainer && data.bookGrid) {
        gridContainer.innerHTML = data.bookGrid;
        initializeFilterHandlers(); // Re-attach event listeners for clear search link
      }

      // Update URL without page refresh
      const newUrl = new URL("/");
      if (data.filter?.searchQuery) {
        newUrl.searchParams.set("q", data.filter.searchQuery);
      }
      if (data.filter?.categoryId) {
        newUrl.searchParams.set("categoryId", data.filter.categoryId);
      }
      if (
        data.filter?.sharingMode !== null &&
        data.filter?.sharingMode !== undefined
      ) {
        newUrl.searchParams.set("mode", data.filter.sharingMode);
      }

      window.history.pushState({ filter: data.filter }, "", newUrl.toString());

      // Update search input
      if (data.filter?.searchQuery !== undefined) {
        const searchInput = document.getElementById("explorer-search-input");
        if (searchInput) {
          searchInput.value = data.filter.searchQuery || "";
        }
      }

      hideLoadingState();
    })
    .catch((error) => {
      console.error("Filter error:", error);
      hideLoadingState();
    });
}

function showLoadingState() {
  const gridContainer = document.getElementById("explorer-grid-container");
  if (gridContainer) {
    gridContainer.style.opacity = "0.6";
    gridContainer.style.pointerEvents = "none";
  }
}

function hideLoadingState() {
  const gridContainer = document.getElementById("explorer-grid-container");
  if (gridContainer) {
    gridContainer.style.opacity = "1";
    gridContainer.style.pointerEvents = "auto";
  }
}
