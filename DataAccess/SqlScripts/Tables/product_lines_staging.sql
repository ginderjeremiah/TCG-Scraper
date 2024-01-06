-- Table: public.product_lines_staging

DROP TABLE IF EXISTS public.product_lines_staging;

CREATE TABLE IF NOT EXISTS public.product_lines_staging
(
    product_line_id integer NOT NULL,
    product_line_name text COLLATE pg_catalog."default",
    product_line_url_name text COLLATE pg_catalog."default",
    CONSTRAINT product_lines_staging_pkey PRIMARY KEY (product_line_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.product_lines_staging
    OWNER to postgres;

GRANT INSERT ON TABLE public.product_lines_staging TO card_reader_uploader;