# TapShow — client API flow

Base: `/api/tapshow/...` · Auth: `Authorization: Bearer <jwt>` on every write. Bodies are JSON unless marked multipart. Errors follow the common envelope (`E204` not found, `E309` not owner, `E000` validation with message). Full reference: `docs/tapshow-area.md`.

## A. Author flow (tạo truyện)

### A1. Tạo post
1. `POST file/upload-media` (multipart `File`, ảnh) → `{ hashId, url, width, height, size }` — thumbnail.
2. `POST tapshow`
   ```json
   { "title": "Bánh mì: Một miếng, hai lựa chọn", "summary": "...", "thumbnailHashId": "<A1.1>",
     "isCurrentUserAuthor": true, "authorName": "", "isMature": false, "isCompleted": false, "permission": 0,
     "tags": ["banhmi", "hai_huoc"],
     "characters": [
       { "name": "VIPHIEN", "avatarHashId": "<upload-media hash|null>", "order": 0 },
       { "name": "Bà bán bánh", "avatarHashId": null, "order": 1 }
     ] }
   ```
   `permission`: 0 Public · 1 Private · 2 Premium. `tags` tuỳ chọn (tối đa 10, chữ / số / `_`, có hoặc không `#`, lưu chữ thường; `PUT tapshow/{hashId}` gửi lại **cả bộ** tag, bỏ tên nào là gỡ tên đó). `characters` tuỳ chọn (tối đa 50), avatar upload trước qua `POST file/upload-media`.
   → `TapShowPostResponse` (`hashId` dùng cho mọi bước sau) kèm `characters[]` `{ id, name, avatarUrl, avatarHashId, order }` — `id` là `characterId` khi tạo segment.

### A2. Nhân vật: thêm / sửa / xoá sau khi tạo
`PUT tapshow/{hashId}` **không** nhận `characters` (gửi cũng bị bỏ qua) — tránh xoá nhầm cả dàn. Mỗi thao tác đụng đúng một nhân vật:
- Thêm: `POST character` `{ "postHashId": "<post>", "name": "...", "avatarHashId": "<hash|null>", "order": null }` (`order` null → cuối danh sách).
- Sửa: `PUT character/{id}` `{ "name", "avatarHashId", "order" }` (`avatarHashId`: giữ nguyên = giữ, mới = thay, null = bỏ avatar).
- Xoá: `DELETE character/{id}` (segment đang dùng giữ nội dung, `characterId` → null).

Danh sách để chọn khi soạn segment: `characters[]` trong `GET tapshow/{hashId}` hoặc `GET character/post/{postHashId}`.

### A3. Tạo chapter
- `POST chapter` `{ "postHashId": "<post>", "title": "Một miếng, hai lựa chọn", "order": null, "status": 0 }`
  `status`: 0 Draft (chỉ owner thấy) · 1 Public. → `{ id, hashId, title, order, status, segmentCount, endingCount, ... }`.
- `PUT chapter/{hashId}` (`title, order, status`) — publish bằng cách đổi `status: 1`.
- `DELETE chapter/{hashId}` — xoá cả segment + choice.
- `GET chapter/post/{postHashId}` — danh sách chapter (owner thấy cả Draft).

### A4. Tạo segment (1 màn hình)
1. `POST file/upload-media` → ảnh segment `hashId` (tuỳ chọn).
2. `POST file/upload-audio` (multipart `File`, mp3/m4a/aac/wav/ogg, ≤ `tapShowAudioSize` MB) → audio `hashId` (tuỳ chọn).
3. `POST segment`
   ```json
   { "chapterHashId": "<chapter>", "characterId": "<A2 id|null>", "title": "Cảnh 14",
     "imageHashId": "<hash|null>", "narration": "Giữ giấy gọn, cắn miếng vừa...", "audioHashId": "<hash|null>", "order": null, "isEnding": false }
   ```
   Cần ít nhất ảnh hoặc `narration`. `order` null → nối cuối; segment `order` nhỏ nhất là màn mở đầu chapter. `isEnding: true` = kết thúc (không Next, không choice).
   → `SegmentResponse` `{ id, chapterId, title, imageUrl, imageHashId, narration, audioUrl, audioHashId, characterId, characterName, characterAvatarUrl, order, isEnding, kind, nextSegmentId, choices: [] }`.
4. `PUT segment/{id}` — cùng body (không có `chapterHashId`). `imageHashId` / `audioHashId`: gửi lại hash cũ = giữ, hash mới = thay, null = bỏ.
5. `DELETE segment/{id}` — xoá luôn các choice trỏ tới nó.

### A5. Rẽ nhánh (sau khi đã có các segment đích)
- `PUT segment/{id}/choices`
  ```json
  { "choices": [ { "label": "Cắn miếng to", "targetSegmentId": "<seg B>" },
                 { "label": "Cắn miếng nhỏ", "targetSegmentId": "<seg C>" } ] }
  ```
  Thay toàn bộ nhánh; thứ tự = thứ tự mảng; tối đa 8; đích phải là segment cùng chapter, không trùng, không tự trỏ. Có choice → segment thành `Choice` (`isEnding` tự về false). `choices: []` → về `Next` thường.
- Truyện tuyến tính KHÔNG cần choice: segment không có choice tự nối sang segment kế theo `order` (nút Next). Chỉ tạo choice ở màn rẽ nhánh; đánh `isEnding: true` ở màn kết.

### A6. Sửa / xoá post
- `PUT tapshow/{hashId}` — body như A1.2 (`thumbnailHashId` cũ = giữ).
- `DELETE tapshow/{hashId}` — xoá cả chapter / segment / choice / nhân vật, file xoá khỏi storage.
- `GET tapshow/my?PageNumber&PageSize` — post của tôi (mọi trạng thái).

## B. Reader flow (đọc truyện, ẩn danh được)

1. `GET tapshow/list?PageNumber=1&PageSize=20&Keyword=&ProfileName=` → paged `TapShowPostResponse` (`chapterCount, commentCount, reaction`).
2. `GET tapshow/{hashId}` → chi tiết post (404 nếu Private / chưa public và không phải owner).
3. `GET chapter/post/{postHashId}` → chapter đã Public, theo `order`.
4. `GET chapter/{chapterHashId}` → `ChapterDetailResponse`
   ```json
   { "id": "...", "hashId": "...", "title": "...", "isLocked": false, "startSegmentId": "<seg>",
     "segments": [ { "id": "...", "imageUrl": "...", "narration": "...", "audioUrl": "...",
                     "characterName": "VIPHIEN", "characterAvatarUrl": "...", "order": 1,
                     "kind": "Choice", "nextSegmentId": null, "isEnding": false,
                     "choices": [ { "id": "...", "label": "...", "order": 0, "targetSegmentId": "..." } ] } ] }
   ```
   Render: bắt đầu ở `startSegmentId`; hiện ảnh, tên + avatar nhân vật, `narration`, phát `audioUrl`. Điều hướng theo `kind`:
   - `Next` → nút Next → `nextSegmentId` (null = hết chapter, gợi ý chapter sau).
   - `Choice` → hiện `choices` làm nút, bấm → `targetSegmentId`.
   - `Ending` → màn kết thúc. Counter "14 / 17" = index trong `segments` / `segments.length`. Toàn bộ graph trong 1 response, không gọi thêm.
   `isLocked: true` (post Premium, viewer không premium/owner) → `segments` rỗng, hiện màn khoá.
5. (tuỳ chọn) `GET character/post/{postHashId}` → dàn nhân vật.

## C. Tương tác (giống Game area)

| Việc | API |
|---|---|
| Comment gốc | `GET comment/post/{postHashId}?PageNumber&PageSize` (mới nhất trước, có `replyCount`, `reaction`) |
| Reply | `GET comment/{id}/replies?PageNumber&PageSize` (cũ nhất trước) |
| Viết comment / reply | `POST comment` `{ "postHashId", "parentId": null|"<root id>", "body" }` (auth) |
| Sửa / xoá comment | `PUT comment/{id}` `{ "body" }` · `DELETE comment/{id}` (author hoặc owner post) |
| Reaction post | `GET reaction/post/{postId}` · `POST reaction/post` `{ "targetId", "type": 0..6 }` · `DELETE reaction/post/{postId}` |
| Reaction comment | `GET reaction/comment/{commentId}` · `POST reaction/comment` · `DELETE reaction/comment/{commentId}` |

`postId` / `commentId` ở reaction là GUID `id`, không phải `hashId`. Reaction response: `{ targetId, totalReacts, currentUserReactType, reactions[{type,count}], mostReactionType }`.

## D. Config cần đọc trước khi upload
`GET /api/identity/config` (route hiện có) → `thumbnailCoverSize` (bytes, ảnh) và `tapShowAudioSize` (bytes, audio) để chặn file quá lớn phía client.

## Thứ tự gọi tối thiểu để có 1 truyện đọc được
upload-media (thumbnail + avatar) → POST tapshow (kèm `characters`) → POST chapter → [upload-media / upload-audio → POST segment] (×n, màn kết `isEnding: true`) → PUT segment/{id}/choices (chỉ ở màn rẽ nhánh) → PUT chapter/{hashId} `status: 1`.
