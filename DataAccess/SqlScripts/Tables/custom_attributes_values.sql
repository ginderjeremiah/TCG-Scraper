-- Table: public.custom_attributes_values

-- DROP TABLE IF EXISTS public.custom_attributes_values;

CREATE TABLE IF NOT EXISTS public.custom_attributes_values
(
    custom_attribute_id integer NOT NULL,
    product_id integer NOT NULL,
    value text COLLATE pg_catalog."default"
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.custom_attributes_values
    OWNER to postgres;
-- Index: ix_custom_attributes_values_product_id

-- DROP INDEX IF EXISTS public.ix_custom_attributes_values_product_id;

CREATE INDEX IF NOT EXISTS ix_custom_attributes_values_product_id
    ON public.custom_attributes_values USING btree
    (product_id ASC NULLS LAST)
    TABLESPACE pg_default;