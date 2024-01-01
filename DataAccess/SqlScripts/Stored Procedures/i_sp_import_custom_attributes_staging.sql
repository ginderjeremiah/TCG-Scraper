-- PROCEDURE: public.i_sp_import_custom_attributes_staging()

-- DROP PROCEDURE IF EXISTS public.i_sp_import_custom_attributes_staging();

CREATE OR REPLACE PROCEDURE public.i_sp_import_custom_attributes_staging(
	)
LANGUAGE 'sql'
    SECURITY DEFINER 
AS $BODY$
	INSERT INTO public.custom_attributes (
		"name",
		display_name,
		product_line_id,
		data_type
	)
	SELECT
		cas."name",
		cas.display_name,
		cas.product_line_id,
		cas.data_type
	FROM public.custom_attributes_staging AS cas
	LEFT JOIN public.custom_attributes AS ca
	ON ca.product_line_id = cas.product_line_id
		AND ca."name" = cas."name"
	WHERE ca.custom_attribute_id IS NULL;

	TRUNCATE public.custom_attributes_staging;
$BODY$;
ALTER PROCEDURE public.i_sp_import_custom_attributes_staging()
    OWNER TO postgres;
