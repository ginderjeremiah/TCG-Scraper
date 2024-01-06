-- Table: public.product_lines

DROP TABLE IF EXISTS public.product_lines;

CREATE TABLE IF NOT EXISTS public.product_lines
(
    product_line_id integer NOT NULL,
    product_line_name text COLLATE pg_catalog."default",
    product_line_url_name text COLLATE pg_catalog."default",
    CONSTRAINT product_lines_pkey PRIMARY KEY (product_line_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.product_lines
    OWNER to postgres;