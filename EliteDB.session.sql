-- INSERT INTO systems (
--     system_name, shortcode, state, control_points, points_change
-- )
-- SELECT
--     system_name, shortcode, state, control_points, points_change
-- FROM powerdata
-- ON DUPLICATE KEY UPDATE
--     shortcode      = VALUES(shortcode),
--     state          = VALUES(state),
--     control_points = VALUES(control_points),
--     points_change  = VALUES(points_change);

ALTER TABLE systems ADD COLUMN has_stronghold_carrier TINYINT(1);

-- CREATE TABLE systems (
--     system_name VARCHAR(50) NOT NULL PRIMARY KEY,
--     latitude FLOAT,
--     longitude FLOAT,
--     height FLOAT,
--     is_anarchy TINYINT(1),
--     state VARCHAR(20),
--     shortcode VARCHAR(4),
--     control_points FLOAT,
--     points_change FLOAT,
-- );

-- INSERT OR REPLACE INTO systems (state, shortcode, control_points, points_change) SELECT state, shortcode, control_points, points_change FROM powerdata WHERE powerdata.system_name = systems.system_name;
-- INSERT INTO systems (state, shortcode, control_points, points_change)
-- SELECT state, shortcode, control_points, points_change 
-- FROM powerdata 
-- ON DUPLICATE KEY UPDATE
--     system_name = VALUES(system_name)
--     state = VALUES(state),
--     shortcode = VALUES(shortcode),
--     control_points = VALUES(control_points),
--     points_change = VALUES(points_change);


-- system_name | varchar(255)
-- latitude    | float
-- longitude   | float
-- height      | float
-- is_anarchy  | tinyint(1)
-- frequency   | int(11)

-- system_name    | varchar(50)
-- shortcode      | varchar(4)
-- state          | varchar(20)
-- control_points | float
-- points_change  | float