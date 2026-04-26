# 📚 Ketabi — Business Requirements Document

> **Version:** 1.0 | **Audience:** Development Team | **Stage:** Pre-MVP

---

## 1. Overview

### Problem
Students and general readers spend significant money on books — especially academic textbooks — that are used briefly and then left on shelves. This leads to financial strain and wasted resources.

### Solution
**Ketabi** is a peer-to-peer platform where users can **list**, **exchange**, and **borrow** books from each other. Physical handoff happens offline; the platform handles discovery, requests, and communication. Initially focused on students and academic books, it's designed to scale to all reader types and categories.

---

## 2. Core Features

- **Book Listing** — Users add books with title, description, condition, category, and transaction type (borrow / exchange).
- **Search & Filtering** — Browse by title, category, or location. Filter by transaction type (borrow or exchange).
- **Borrow Flow** — Request a book for a defined duration. Owner approves or rejects.
- **Exchange Flow** — Offer one of your own books in return. Owner approves or rejects.
- **In-App Chat** — Opens automatically after a request is accepted. Used to coordinate the offline meetup.
- **Transaction Completion** — Both parties confirm physical handoff. Book status updates accordingly.
- **Ratings & Reviews** — Users rate each other after a completed transaction.

---

## 3. System Flow

```
[Register / Login]
      ↓
[Add Book] → Enter title, condition, category → Select: Borrow | Exchange → Book goes live
      ↓
[Search / Browse] → Filter by type or location → View book details
      ↓
[Send Request]
  • Borrow → Select duration → Submit
  • Exchange → Pick one of your books → Submit
      ↓
[Owner Reviews Request] → Accept ✅ or Reject ❌
      ↓
[Chat Opens] → Agree on meeting point & time
      ↓
[Physical Exchange Happens]
      ↓
[Both Confirm Completion] → Book status updates → Leave a review
```

---

## 4. Key Use Cases

### UC-01 · Register & Set Up Profile
**Actor:** Guest
User creates an account, sets location and areas of interest. Location improves matching quality.

**Flow:** Visit site → Register → Complete profile (location, interests) → Access marketplace

---

### UC-02 · List a Book
**Actor:** User
User makes a book available to others for borrowing or exchange.

**Flow:** Click "Add Book" → Fill in details (title, condition, category) → Select transaction type → Set borrow duration or preferred exchange → Publish

---

### UC-03 · Search for a Book
**Actor:** User / Guest
Find available books using search and filters.

**Flow:** Enter title or category → Apply filters (borrow/exchange, location) → View listing details

---

### UC-04 · Request to Borrow
**Actor:** User
User requests a listed book for a specific time period.

**Flow:** Open book listing → Click "Request Borrow" → Select duration → Send request → Wait for owner response

---

### UC-05 · Request an Exchange
**Actor:** User
User proposes their own book in return for a listed book.

**Flow:** Open book listing → Click "Request Exchange" → Select one of their listed books → Send request → Wait for owner response

---

### UC-06 · Handle a Request (Owner)
**Actor:** User (Book Owner)
Owner reviews incoming requests and decides to accept or reject.

**Flow:** Receive notification → View request details → Accept ✅ or Reject ❌ → If accepted, chat opens automatically

---

### UC-07 · Complete a Transaction
**Actor:** Both Users
After meeting offline, both parties confirm the exchange or borrow happened.

**Flow:** Both click "Confirm Completion" → Book status updates (Borrowed / Exchanged) → Review prompt appears

---

### UC-08 · Admin — Manage Platform
**Actor:** Admin
Monitor users, listings, and flag/remove inappropriate content.

**Flow:** Access dashboard → View all listings/users → Suspend or remove content → Review reports

---

## 5. Business Rules

- A book belongs to one owner at a time.
- Only the book owner can accept or reject requests.
- A book can only have one active request at a time.
- Book statuses: **Available → Reserved → Borrowed / Exchanged → Available**
- Users have a cap on the number of simultaneous active requests (e.g., max 3).
- Both users must confirm completion for the transaction to close.
- Unconfirmed transactions auto-expire after a set period (future scope).
- Guests can browse but cannot list, request, or chat.

---

## 6. Data Model Overview

| Entity | Description |
|---|---|
| **User** | Registered account with profile, location, and reputation score |
| **Book** | Listed item with title, condition, category, and transaction type |
| **Request** | A borrow or exchange proposal from one user to a book owner |
| **Chat / Message** | Conversation thread linked to an accepted request |
| **Review** | Rating and comment left after a completed transaction |

---

## 7. MVP Scope

### ✅ Build Now
- User registration and login (email/password)
- Book listing (borrow & exchange)
- Search and category filtering
- Borrow and exchange request flows
- Request accept / reject
- In-app chat (post-acceptance)
- Transaction confirmation by both parties
- Basic user profiles (name, location)
- Simple admin panel (view/delete listings and users)

### 🔜 Post-MVP
- Ratings and reviews
- Notifications (in-app and email)
- Advanced filters (by location radius)
- Request caps and auto-expiry logic
- Featured listings / monetization layer

---

## 8. Future Enhancements

- **Location-based matching** — Show books near the user first
- **Recommendation engine** — Suggest books based on user interests
- **Notification system** — Push / email alerts for requests and messages
- **Subscription plans** — Unlock priority listings or extended borrow limits
- **Monetization** — Commission per completed transaction, promoted listings
- **Mobile app** — Native iOS / Android clients
- **Trust & safety** — ID verification, dispute resolution flow
