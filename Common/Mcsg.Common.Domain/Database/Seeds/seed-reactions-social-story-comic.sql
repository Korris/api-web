-- =====================================================================
-- Seed fake reactions for Social / Story / Comic
-- Targets: posts, sub-posts (chapters), post comments, sub-post comments
--
-- Run:  psql -h localhost -U postgres -d dev_focfoc -f seed-reactions-social-story-comic.sql
--       (or paste the WHOLE file into pgAdmin / DBeaver and execute as script)
--
-- Rules respected:
--   * one reaction per (TargetId, AuthorId) -- same as ReactService (upsert per user)
--   * Type = ReactionType 0..6 (Like1..Like7), skewed so Like1 is most common
--   * ParentId left NULL (service does not set it either)
-- All seeded rows are tagged "TagData" = 'seed:reactions' for easy cleanup.
-- Requires PostgreSQL 13+ (gen_random_uuid built-in).
-- =====================================================================

BEGIN;

-- ---------------------------------------------------------------------
-- 1. Generic seeding function (dynamic SQL, one call per reaction table)
--    p_schema          : social | story | comic
--    p_reaction_table  : e.g. SocialPostReactions
--    p_target_table    : table that "TargetId" points to (SocialPosts, SocialPostComments, ...)
--    p_target_filter   : extra WHERE for targets ('"Status" = 1' for posts, '"Status" = 1' for comments)
--    p_max_targets     : how many random targets get reactions
--    p_min_per_target  : min reactions per target
--    p_max_per_target  : max reactions per target (capped by number of users)
-- ---------------------------------------------------------------------
CREATE OR REPLACE FUNCTION pg_temp.seed_reactions(
    p_schema text, p_reaction_table text, p_target_table text, p_target_filter text,
    p_max_targets int, p_min_per_target int, p_max_per_target int)
RETURNS int LANGUAGE plpgsql AS $fn$
DECLARE
    v_inserted int;
BEGIN
    EXECUTE format($q$
        WITH users AS (
            SELECT "Id" FROM identity."Users" WHERE "IsDelete" = false
        ),
        targets AS (
            SELECT "Id",
                   -- random reaction count in [min, max] for this target
                   (%6$s + floor(random() * (%7$s - %6$s + 1)))::int AS n
            FROM %1$I.%3$I
            WHERE "IsDelete" = false AND (%4$s)
            ORDER BY random() LIMIT %5$s
        ),
        picked AS (
            -- distinct random users per target (LATERAL re-evaluates per target row)
            SELECT t."Id" AS target_id, u."Id" AS user_id
            FROM targets t
            CROSS JOIN LATERAL (
                SELECT "Id" FROM users
                WHERE t."Id" IS NOT NULL          -- correlation so random() re-rolls per target
                ORDER BY random() LIMIT t.n
            ) u
        ),
        ins AS (
            INSERT INTO %1$I.%2$I
                ("Id","TargetId","AuthorId","Type","CreatedBy","CreatedOn","IsDelete","TagData")
            SELECT gen_random_uuid(),
                   p.target_id,
                   p.user_id,
                   -- skewed: power() pushes most rolls toward 0 (Like1), few reach 6 (Like7)
                   LEAST(6, floor(power(random(), 2.5) * 7))::int,
                   p.user_id,
                   now() - random() * interval '30 days',
                   false,
                   'seed:reactions'
            FROM picked p
            WHERE NOT EXISTS (                    -- keep 1 reaction per user per target
                SELECT 1 FROM %1$I.%2$I r
                WHERE r."TargetId" = p.target_id AND r."AuthorId" = p.user_id AND r."IsDelete" = false
            )
            RETURNING 1
        )
        SELECT count(*) FROM ins
    $q$, p_schema, p_reaction_table, p_target_table, p_target_filter,
         p_max_targets, p_min_per_target, p_max_per_target)
    INTO v_inserted;

    RAISE NOTICE '%.% -> % rows', p_schema, p_reaction_table, v_inserted;
    RETURN v_inserted;
END $fn$;

-- ---------------------------------------------------------------------
-- 2. Run: (schema, reaction table, target table, target filter, max targets, min/target, max/target)
--    Status = 1 means Public for both PostStatus and CommentStatus.
--    Adjust numbers to taste.
-- ---------------------------------------------------------------------
-- Posts
SELECT pg_temp.seed_reactions('social', 'SocialPostReactions', 'SocialPosts', '"Status" = 1', 80, 3, 25);
SELECT pg_temp.seed_reactions('story',  'StoryPostReactions',  'StoryPosts',  '"Status" = 1', 80, 3, 25);
SELECT pg_temp.seed_reactions('comic',  'ComicPostReactions',  'ComicPosts',  '"Status" = 1', 80, 3, 25);

-- Sub-posts / chapters
SELECT pg_temp.seed_reactions('social', 'SocialSubPostReactions', 'SocialSubPosts', '"Status" = 1', 100, 1, 15);
SELECT pg_temp.seed_reactions('story',  'StorySubPostReactions',  'StorySubPosts',  '"Status" = 1', 150, 1, 15);
SELECT pg_temp.seed_reactions('comic',  'ComicSubPostReactions',  'ComicSubPosts',  '"Status" = 1', 150, 1, 15);

-- Post comments
SELECT pg_temp.seed_reactions('social', 'SocialPostCommentReactions', 'SocialPostComments', '"Status" = 1', 300, 0, 8);
SELECT pg_temp.seed_reactions('story',  'StoryPostCommentReactions',  'StoryPostComments',  '"Status" = 1', 300, 0, 8);
SELECT pg_temp.seed_reactions('comic',  'ComicPostCommentReactions',  'ComicPostComments',  '"Status" = 1', 300, 0, 8);

-- Sub-post comments
SELECT pg_temp.seed_reactions('social', 'SocialSubPostCommentReactions', 'SocialSubPostComments', '"Status" = 1', 300, 0, 8);
SELECT pg_temp.seed_reactions('story',  'StorySubPostCommentReactions',  'StorySubPostComments',  '"Status" = 1', 300, 0, 8);
SELECT pg_temp.seed_reactions('comic',  'ComicSubPostCommentReactions',  'ComicSubPostComments',  '"Status" = 1', 300, 0, 8);

COMMIT;

-- ---------------------------------------------------------------------
-- CLEANUP (run separately if you want to remove seeded reactions)
-- ---------------------------------------------------------------------
-- DELETE FROM social."SocialPostReactions"           WHERE "TagData" = 'seed:reactions';
-- DELETE FROM social."SocialSubPostReactions"        WHERE "TagData" = 'seed:reactions';
-- DELETE FROM social."SocialPostCommentReactions"    WHERE "TagData" = 'seed:reactions';
-- DELETE FROM social."SocialSubPostCommentReactions" WHERE "TagData" = 'seed:reactions';
-- DELETE FROM story."StoryPostReactions"             WHERE "TagData" = 'seed:reactions';
-- DELETE FROM story."StorySubPostReactions"          WHERE "TagData" = 'seed:reactions';
-- DELETE FROM story."StoryPostCommentReactions"      WHERE "TagData" = 'seed:reactions';
-- DELETE FROM story."StorySubPostCommentReactions"   WHERE "TagData" = 'seed:reactions';
-- DELETE FROM comic."ComicPostReactions"             WHERE "TagData" = 'seed:reactions';
-- DELETE FROM comic."ComicSubPostReactions"          WHERE "TagData" = 'seed:reactions';
-- DELETE FROM comic."ComicPostCommentReactions"      WHERE "TagData" = 'seed:reactions';
-- DELETE FROM comic."ComicSubPostCommentReactions"   WHERE "TagData" = 'seed:reactions';
