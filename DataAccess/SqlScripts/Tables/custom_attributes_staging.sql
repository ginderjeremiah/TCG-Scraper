-- Table: public.custom_attributes

DROP TABLE IF EXISTS public.custom_attributes_staging;

CREATE TABLE IF NOT EXISTS public.custom_attributes_staging
(
    "name" text COLLATE pg_catalog."default" NOT NULL,
	display_name text COLLATE pg_catalog."default" NOT NULL,
	product_line_id INT NOT NULL,
	data_type TEXT COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT custom_attributes_staging_pkey PRIMARY KEY (product_line_id, "name")
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.custom_attributes_staging
    OWNER to postgres;
	
GRANT INSERT ON TABLE public.custom_attributes_staging TO card_reader_uploader