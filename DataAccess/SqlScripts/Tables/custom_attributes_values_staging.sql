-- Table: public.custom_attributes_values_staging

DROP TABLE IF EXISTS public.custom_attributes_values_staging;

CREATE TABLE IF NOT EXISTS public.custom_attributes_values_staging
(
    custom_attribute_id integer NOT NULL,
    product_id integer NOT NULL,
    value text COLLATE pg_catalog."default",
    CONSTRAINT custom_attributes_values_staging_pkey PRIMARY KEY (product_id, custom_attribute_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.custom_attributes_values_staging
    OWNER to postgres;
	
GRANT INSERT ON TABLE public.custom_attributes_values_staging TO card_reader_uploader;