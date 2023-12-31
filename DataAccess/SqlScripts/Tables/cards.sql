-- Table: public.cards

-- DROP TABLE IF EXISTS public.cards;

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
    description text COLLATE pg_catalog."default",
    detail_note text COLLATE pg_catalog."default",
    intellect text COLLATE pg_catalog."default",
    release_date timestamp without time zone,
    "number" text COLLATE pg_catalog."default",
    talent text COLLATE pg_catalog."default",
    pitch_value smallint,
    card_type text COLLATE pg_catalog."default" NOT NULL,
    defense_value smallint,
    rarity_dbname text COLLATE pg_catalog."default",
    life text COLLATE pg_catalog."default",
    card_subtype text COLLATE pg_catalog."default",
    power text COLLATE pg_catalog."default",
    flavor_text text COLLATE pg_catalog."default",
    class text COLLATE pg_catalog."default" NOT NULL,
    cost smallint,
    CONSTRAINT cards_pkey PRIMARY KEY (product_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.cards
    OWNER to postgres;

REVOKE ALL ON TABLE public.cards FROM cardreaderuploader;

GRANT UPDATE, INSERT, SELECT ON TABLE public.cards TO cardreaderuploader;

GRANT ALL ON TABLE public.cards TO postgres;