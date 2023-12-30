-- Table: public.card_listings

-- DROP TABLE IF EXISTS public.card_listings;

CREATE TABLE IF NOT EXISTS public.card_listings
(
    direct_product text COLLATE pg_catalog."default" NOT NULL,
    gold_seller boolean NOT NULL,
    listing_id integer NOT NULL,
    channel_id boolean NOT NULL,
    condition_id integer NOT NULL,
    verified_seller boolean NOT NULL,
    direct_inventory integer NOT NULL,
    ranked_shipping_price real NOT NULL,
    product_id integer NOT NULL,
    printing text COLLATE pg_catalog."default" NOT NULL,
    language_abbreviation text COLLATE pg_catalog."default" NOT NULL,
    seller_name text COLLATE pg_catalog."default" NOT NULL,
    forward_freight text COLLATE pg_catalog."default" NOT NULL,
    seller_shipping_price real NOT NULL,
    language text COLLATE pg_catalog."default" NOT NULL,
    shipping_price real NOT NULL,
    condition text COLLATE pg_catalog."default" NOT NULL,
    language_id integer NOT NULL,
    score real NOT NULL,
    direct_seller boolean NOT NULL,
    product_condition_id integer NOT NULL,
    seller_id integer NOT NULL,
    listing_type text COLLATE pg_catalog."default" NOT NULL,
    seller_rating real NOT NULL,
    seller_sales integer,
    quantity integer NOT NULL,
    seller_key text COLLATE pg_catalog."default" NOT NULL,
    price real NOT NULL,
    custom_data text COLLATE pg_catalog."default",
    CONSTRAINT card_listings_pkey PRIMARY KEY (listing_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.card_listings
    OWNER to postgres;

REVOKE ALL ON TABLE public.card_listings FROM cardreaderuploader;

GRANT UPDATE, INSERT, SELECT ON TABLE public.card_listings TO cardreaderuploader;

GRANT ALL ON TABLE public.card_listings TO postgres;