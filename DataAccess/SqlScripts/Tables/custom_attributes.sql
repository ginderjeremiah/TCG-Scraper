-- Table: public.custom_attributes

-- DROP TABLE IF EXISTS public.custom_attributes;

CREATE TABLE IF NOT EXISTS public.custom_attributes
(
    custom_attribute_id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    name text COLLATE pg_catalog."default",
    CONSTRAINT custom_attributes_pkey PRIMARY KEY (custom_attribute_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.custom_attributes
    OWNER to postgres;