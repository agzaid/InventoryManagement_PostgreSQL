select * from items;

update items set recall_qnt = 1 WHERE item_code = '0402';

select * from branches;
select * from departments;
select * from emp_egx;
select * from inv_trans;
select * from inv_users;
select * from items;
select * from item_balance;
select * from item_card;
select * from item_category;
select * from monthly_consum;
select * from monthly_balance;
select * from open_balance;
select * from stores;
select * from supplier;

select * from kwarehouse."__EFMigrationsHistory";
DROP TABLE kwarehouse."__EFMigrationsHistory";

GRANT ALL PRIVILEGES ON DATABASE postgres TO kwarehouse;
GRANT ALL ON SCHEMA public TO kwarehouse;
GRANT CREATE ON SCHEMA public TO kwarehouse;

DROP TABLE IF EXISTS public."__EFMigrationsHistory";

ALTER TABLE kwarehouse.branches OWNER TO kwarehouse;

SELECT setval(pg_get_serial_sequence('kwarehouse.branches', 'Id'), max("Id"))
FROM kwarehouse.branches;
SELECT setval(pg_get_serial_sequence('kwarehouse.departments', 'id'), max(id)) FROM kwarehouse.departments;
SELECT setval(
    pg_get_serial_sequence('kwarehouse.emp_egx', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.emp_egx), 0) + 1,
    false
);
SELECT setval(
    pg_get_serial_sequence('kwarehouse.inv_trans', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.inv_trans), 0) + 1,
    false
);
SELECT setval(
    pg_get_serial_sequence('kwarehouse.inv_users', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.inv_users), 0) + 1,
    false
);
-- Reset item table
SELECT setval(pg_get_serial_sequence('kwarehouse.items', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.items), 0) + 1, false);

-- Reset item_balance table
SELECT setval(pg_get_serial_sequence('kwarehouse.item_balance', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.item_balance), 0) + 1, false);

-- Reset item_card table
SELECT setval(pg_get_serial_sequence('kwarehouse.item_card', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.item_card), 0) + 1, false);

-- Reset item_category table
SELECT setval(pg_get_serial_sequence('kwarehouse.item_category', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.item_category), 0) + 1, false);

-- Reset monthly_balance table
SELECT setval(pg_get_serial_sequence('kwarehouse.monthly_balance', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.monthly_balance), 0) + 1, false);

-- Reset monthly_consum table
SELECT setval(pg_get_serial_sequence('kwarehouse.monthly_consum', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.monthly_consum), 0) + 1, false);

-- Reset open_balance table
SELECT setval(pg_get_serial_sequence('kwarehouse.open_balance', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.open_balance), 0) + 1, false);
SELECT setval(pg_get_serial_sequence('kwarehouse.supplier', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.supplier), 0) + 1, false);
SELECT setval(
    pg_get_serial_sequence('kwarehouse.stores', 'id'),
    COALESCE((SELECT MAX(id) FROM kwarehouse.stores), 0) + 1,
    false
);


SELECT constraint_name
FROM information_schema.table_constraints
WHERE table_name = 'departments'
  AND table_schema = 'kwarehouse'
  AND constraint_type = 'PRIMARY KEY';

SELECT setval(pg_get_serial_sequence('kwarehouse.branches', 'id'), max(id))
FROM kwarehouse.branches;

SELECT constraint_name
FROM information_schema.key_column_usage
WHERE table_name = 'inv_trans' AND table_schema = 'kwarehouse';