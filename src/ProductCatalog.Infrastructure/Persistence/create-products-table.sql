CREATE TABLE IF NOT EXISTS products (
    product_id uuid PRIMARY KEY,
    name varchar(120) NOT NULL,
    description varchar(1000) NOT NULL,
    unit_price numeric(12, 2) NOT NULL CHECK (unit_price >= 0),
    stock_quantity integer NOT NULL CHECK (stock_quantity >= 0),
    created_at_utc timestamptz NOT NULL,
    updated_at_utc timestamptz NOT NULL
);
