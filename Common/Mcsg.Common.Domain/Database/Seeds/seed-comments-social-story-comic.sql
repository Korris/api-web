-- =====================================================================
-- Seed fake Vietnamese comments for Social / Story / Comic
-- (post-level + sub-post/chapter-level, incl. replies)
--
-- Run:  psql -h localhost -U postgres -d dev_focfoc -f seed-comments-social-story-comic.sql
--       (or paste into pgAdmin / DBeaver query editor and execute)
--
-- All seeded rows are tagged "TagData" = 'seed:comments' so they can be
-- removed again with the CLEANUP block at the bottom of this file.
-- Requires PostgreSQL 13+ (gen_random_uuid built-in).
-- =====================================================================

BEGIN;

-- ---------------------------------------------------------------------
-- 1. Comment bodies (kind = 'top' for root comments, 'reply' for replies)
-- ---------------------------------------------------------------------
DROP TABLE IF EXISTS tmp_seed_comment_bodies;
CREATE TEMP TABLE tmp_seed_comment_bodies (kind text, body text) ON COMMIT DROP;

INSERT INTO tmp_seed_comment_bodies (kind, body) VALUES
-- ===== ROOT COMMENTS: bàn tán về truyện / chương =====
('top', 'Chương này hay quá, đọc xong mà tim đập thình thịch luôn 😭'),
('top', 'Ai đọc raw rồi cho hỏi main có bị gì không, hồi hộp quá'),
('top', 'Tác giả ra chương đều ghê, tuần nào cũng có, tôn trọng'),
('top', 'Plot twist chương này khét thật sự, không đoán được luôn'),
('top', 'Đọc lại lần 3 rồi mà vẫn thấy hay như lần đầu'),
('top', 'Nữ chính chương này đáng yêu quá đi, chịu không nổi 🥹'),
('top', 'Có ai thấy nhân vật phụ này đáng nghi không? Kiểu gì cũng phản diện'),
('top', 'Chờ chương mới dài cổ luôn, tác giả ơi ra nhanh với'),
('top', 'Art càng ngày càng đẹp, khác hẳn mấy chương đầu'),
('top', 'Bộ này mới đọc từ hôm qua mà giờ cày tới sáng rồi 😵'),
('top', 'Đoạn cuối chương cliffhanger ác thật, tuần sau mới có chương tiếp'),
('top', 'Cảm giác tác giả đang xây dựng cái gì đó lớn lắm, mấy chương gần đây toàn foreshadowing'),
('top', 'Mình đoán ông thầy là người đứng sau hết mọi chuyện. Ai cùng ý kiến không?'),
('top', 'Chương này hơi lan man, mong tác giả đẩy nhanh nhịp chút'),
('top', 'Đọc xong chương này mà buồn cả ngày, không nghĩ nhân vật đó đi sớm vậy'),
('top', 'Bạn nào có link đọc bản đầy đủ không, trên này thiếu mấy trang'),
('top', 'Cảnh đánh nhau vẽ đỉnh quá, nhìn từng khung mà thấy chuyển động luôn'),
('top', 'Bộ này bị đánh giá thấp thật, đáng ra phải nổi hơn nhiều'),
('top', 'Nhân vật main phát triển tâm lý hợp lý, không bị kiểu bỗng dưng mạnh lên'),
('top', 'Lâu lắm rồi mới có bộ khiến mình chờ chương từng tuần như vậy'),
('top', 'Đọc tới đoạn hai đứa gặp lại nhau mà nổi da gà 😭😭'),
('top', 'Thấy có người bảo sắp end rồi, không biết thật không?'),
('top', 'Chương này ít thoại quá, đọc 2 phút là hết'),
('top', 'Ship cặp này từ chương 5 tới giờ, cuối cùng cũng có tiến triển'),
('top', 'Mấy bạn đọc kỹ đoạn hồi tưởng chưa, có chi tiết rất quan trọng đó'),
('top', 'Ai giải thích giúp mình đoạn cuối với, đọc không hiểu lắm'),
('top', 'Bộ này giống bộ kia ghê, mà vẫn hay theo cách riêng'),
('top', 'Chương này cười muốn xỉu, đoạn ăn cơm là đỉnh nhất 🤣'),
('top', 'Tác giả viết thoại thông minh quá, đọc thấy đã'),
('top', 'Nhìn bìa chương là biết chương này có biến rồi'),
('top', 'Ngồi canh 12h đêm để đọc chương mới, giờ đọc xong lại ngồi chờ tiếp'),
('top', 'Cốt truyện càng về sau càng chặt, đầu tư ghê'),
('top', 'Có bạn nào tổng hợp timeline bộ này chưa, nhiều tuyến quá đọc rối'),
('top', 'Chương này chắc nhiều người khóc lắm đây'),
('top', 'Mới đọc chương đầu thôi mà thấy hứa hẹn ghê, follow luôn'),
('top', 'Cảm ơn nhóm dịch, dịch mượt và ra nhanh quá'),
('top', 'Đọc chương này xong tự nhiên muốn đọc lại từ đầu'),
('top', 'Phản diện bộ này thuyết phục thật, không phải kiểu ác vô lý'),
('top', 'Mong có bản in để mua ủng hộ tác giả'),
('top', 'Câu thoại cuối chương ám ảnh thật, đọc xong cứ nghĩ mãi'),
-- ===== ROOT COMMENTS: bàn tán kiểu tin tức / social =====
('top', 'Tin này thật không vậy, thấy nhiều nguồn nói khác nhau quá'),
('top', 'Đọc xong bài này thấy đúng là thời buổi gì cũng có thể xảy ra'),
('top', 'Chia sẻ cho mọi người cùng đọc, bài viết rất đáng suy ngẫm'),
('top', 'Mình đã nghi từ lâu rồi, giờ mới có bài nói rõ'),
('top', 'Bài dài nhưng đáng đọc, cảm ơn tác giả'),
('top', 'Ai có thêm thông tin về vụ này không, hóng quá'),
('top', 'Cái này hôm qua mình vừa thấy ngoài đời luôn, đúng thật'),
('top', 'Theo dõi vụ này từ đầu, đúng là càng ngày càng rối'),
('top', 'Không đồng ý lắm với quan điểm bài viết nhưng vẫn respect'),
('top', 'Bài này viết dễ hiểu, đọc cái hiểu luôn 👍'),
('top', 'Đọc xong mà thấy lo lo, không biết sắp tới sao nữa'),
('top', 'Tin hot vậy mà giờ mới thấy, cảm ơn đã đăng'),
('top', 'Hóng phần tiếp theo của bài này'),
('top', 'Mọi người bình tĩnh, chờ thông tin chính thức đã'),
('top', 'Bài viết hay, nhưng ảnh minh họa hơi không liên quan 😅'),
-- ===== REPLIES =====
('reply', 'Đồng ý luôn, mình cũng nghĩ vậy'),
('reply', 'Không đâu bạn, đọc kỹ lại chương trước đi'),
('reply', 'Chuẩn, mình cũng đoán y chang'),
('reply', 'Spoil nhẹ: chương sau còn hay hơn nữa 😏'),
('reply', 'Bạn đọc raw à, xin link với'),
('reply', 'Haha đúng rồi, mình cũng thức tới sáng vì bộ này'),
('reply', 'Đừng spoil nữa, mình chưa đọc tới đó 😭'),
('reply', 'Mình thấy bình thường thôi, không hay như mọi người nói'),
('reply', 'Cùng cảm nhận, đoạn đó mình cũng khóc'),
('reply', '+1, tuần nào cũng canh chương mới'),
('reply', 'Ủa vậy hả, mình lại thấy ngược lại'),
('reply', 'Chắc là vậy đó, tác giả gợi ý từ mấy chương trước rồi'),
('reply', 'Cảm ơn bạn giải thích, giờ mình hiểu rồi'),
('reply', 'Thật hả, không ngờ luôn'),
('reply', 'Đúng người đúng thời điểm 🤣'),
('reply', 'Mình nghĩ bạn nhầm nhân vật rồi'),
('reply', 'Đọc lại lần nữa mới thấy chi tiết bạn nói, hay thật'),
('reply', 'Bạn nói đúng, chương này nhịp hơi chậm'),
('reply', 'Không biết bao giờ mới có chương mới nữa 😢'),
('reply', 'Cùng ship cặp này 🙌'),
('reply', 'Mình thì đoán là em gái mới là người đứng sau'),
('reply', 'Chắc phải đọc lại từ đầu mới hiểu hết'),
('reply', 'Đọc comment bạn xong mình càng hóng hơn'),
('reply', 'Đúng, nguồn đó không đáng tin lắm'),
('reply', 'Chờ thêm tin chính thức đi bạn'),
('reply', 'Mình cũng thấy vậy, thông tin còn mâu thuẫn'),
('reply', 'Cảm ơn bạn đã chia sẻ thêm'),
('reply', 'Cái này mình biết từ lâu rồi mà'),
('reply', 'Hóng cùng bạn'),
('reply', 'Ok bạn, để mình đọc lại xem sao');

-- ---------------------------------------------------------------------
-- 2. Generic seeding function (dynamic SQL, one call per comment table)
--    p_schema         : social | story | comic
--    p_comment_table  : e.g. SocialPostComments
--    p_post_table     : e.g. SocialPosts (parent table for "PostId")
--    p_max_posts      : how many random public posts to comment on
--    p_per_post       : root comments per post
--    p_reply_chance   : 0..1 chance each root comment gets a reply
-- ---------------------------------------------------------------------
CREATE OR REPLACE FUNCTION pg_temp.seed_comments(
    p_schema text, p_comment_table text, p_post_table text,
    p_max_posts int, p_per_post int, p_reply_chance numeric)
RETURNS int LANGUAGE plpgsql AS $fn$
DECLARE
    v_inserted int;
BEGIN
    EXECUTE format($q$
        WITH cfg AS (
            SELECT (SELECT array_agg("Id") FROM identity."Users" WHERE "IsDelete" = false)          AS users,
                   (SELECT array_agg(body) FROM tmp_seed_comment_bodies WHERE kind = 'top')       AS tops,
                   (SELECT array_agg(body) FROM tmp_seed_comment_bodies WHERE kind = 'reply')     AS replies
        ),
        posts AS (
            SELECT "Id" FROM %1$I.%3$I
            WHERE "IsDelete" = false AND "Status" = 1      -- PostStatus.Public
            ORDER BY random() LIMIT %4$s
        ),
        raw_top AS (
            SELECT gen_random_uuid()                                                   AS id,
                   cfg.tops[1 + floor(random() * cardinality(cfg.tops))::int]          AS body,
                   p."Id"                                                              AS post_id,
                   cfg.users[1 + floor(random() * cardinality(cfg.users))::int]        AS user_id,
                   now() - random() * interval '30 days'                               AS created_on
            FROM posts p CROSS JOIN cfg CROSS JOIN generate_series(1, %5$s) g
        ),
        ins_top AS (
            INSERT INTO %1$I.%2$I
                ("Id","Body","PostId","AuthorId","CreatedBy","CreatedOn","Order","Status","IsDelete","TagData")
            SELECT id, body, post_id, user_id, user_id, created_on, 0, 1 /*CommentStatus.Public*/, false, 'seed:comments'
            FROM raw_top
            RETURNING "Id", "PostId", "CreatedOn"
        ),
        raw_reply AS (
            SELECT gen_random_uuid()                                                   AS id,
                   cfg.replies[1 + floor(random() * cardinality(cfg.replies))::int]    AS body,
                   t."PostId"                                                          AS post_id,
                   t."Id"                                                              AS parent_id,
                   cfg.users[1 + floor(random() * cardinality(cfg.users))::int]        AS user_id,
                   t."CreatedOn" + random() * interval '2 days'                        AS created_on,
                   random()                                                            AS dice
            FROM ins_top t CROSS JOIN cfg
        ),
        ins_reply AS (
            INSERT INTO %1$I.%2$I
                ("Id","Body","PostId","ParentId","AuthorId","CreatedBy","CreatedOn","Order","Status","IsDelete","TagData")
            SELECT id, body, post_id, parent_id, user_id, user_id, created_on, 0, 1, false, 'seed:comments'
            FROM raw_reply WHERE dice < %6$s
            RETURNING 1
        )
        SELECT (SELECT count(*) FROM ins_top) + (SELECT count(*) FROM ins_reply)
    $q$, p_schema, p_comment_table, p_post_table, p_max_posts, p_per_post, p_reply_chance)
    INTO v_inserted;

    RAISE NOTICE '%.% -> % rows', p_schema, p_comment_table, v_inserted;
    RETURN v_inserted;
END $fn$;

-- ---------------------------------------------------------------------
-- 3. Run: (schema, comment table, parent post table, max posts, per post, reply chance)
--    Adjust numbers to taste.
-- ---------------------------------------------------------------------
SELECT pg_temp.seed_comments('social', 'SocialPostComments',    'SocialPosts',    50, 6, 0.5);
SELECT pg_temp.seed_comments('social', 'SocialSubPostComments', 'SocialSubPosts', 50, 4, 0.4);
SELECT pg_temp.seed_comments('story',  'StoryPostComments',     'StoryPosts',     50, 6, 0.5);
SELECT pg_temp.seed_comments('story',  'StorySubPostComments',  'StorySubPosts',  80, 5, 0.5);
SELECT pg_temp.seed_comments('comic',  'ComicPostComments',     'ComicPosts',     50, 6, 0.5);
SELECT pg_temp.seed_comments('comic',  'ComicSubPostComments',  'ComicSubPosts',  80, 5, 0.5);

COMMIT;

-- ---------------------------------------------------------------------
-- CLEANUP (run separately if you want to remove seeded comments)
-- ---------------------------------------------------------------------
-- DELETE FROM social."SocialPostComments"    WHERE "TagData" = 'seed:comments';
-- DELETE FROM social."SocialSubPostComments" WHERE "TagData" = 'seed:comments';
-- DELETE FROM story."StoryPostComments"      WHERE "TagData" = 'seed:comments';
-- DELETE FROM story."StorySubPostComments"   WHERE "TagData" = 'seed:comments';
-- DELETE FROM comic."ComicPostComments"      WHERE "TagData" = 'seed:comments';
-- DELETE FROM comic."ComicSubPostComments"   WHERE "TagData" = 'seed:comments';
