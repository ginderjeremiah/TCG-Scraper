-- Table: public.cards

DROP TABLE IF EXISTS public.cards;

CREATE TABLE IF NOT EXISTS public.cards
(
    shipping_category_id integer NOT NULL,
    duplicate boolean NOT NULL,
    product_line_url_name text COLLATE pg_catalog."default" NOT NULL,
    product_url_name text COLLATE pg_catalog."default" NOT NULL,
    product_type_id integer NOT NULL,
    rarity_name text COLLATE pg_catalog."default",
    sealed boolean NOT NULL,
    market_price real NOT NULL,
    lowest_price_with_shipping real NOT NULL,
    product_name text COLLATE pg_catalog."default" NOT NULL,
    set_id integer NOT NULL,
    product_id integer NOT NULL,
    score real,
    set_name text COLLATE pg_catalog."default" NOT NULL,
    foil_only boolean NOT NULL,
    set_url_name text COLLATE pg_catalog."default" NOT NULL,
    seller_listable boolean NOT NULL,
    total_listings integer NOT NULL,
    product_line_id integer NOT NULL,
    product_status_id integer NOT NULL,
    product_line_name text COLLATE pg_catalog."default" NOT NULL,
    max_fulfullable_quantity integer NOT NULL,
    lowest_price real NOT NULL,
    CONSTRAINT cards_pkey PRIMARY KEY (product_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.cards
    OWNER to postgres;