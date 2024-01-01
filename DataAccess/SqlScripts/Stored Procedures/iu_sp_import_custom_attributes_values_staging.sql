-- PROCEDURE: public.iu_sp_import_custom_attributes_values_staging()

-- DROP PROCEDURE IF EXISTS public.iu_sp_import_custom_attributes_values_staging();

CREATE OR REPLACE PROCEDURE public.iu_sp_import_custom_attributes_values_staging(
	)
LANGUAGE 'sql'
    SECURITY DEFINER 
AS $BODY$
	UPDATE public.custom_attributes_values AS cav
		SET "value" = cavs."value"
	FROM public.custom_attributes_values_staging AS cavs
	WHERE cav.product_id = cavs.product_id
		AND cav.custom_attribute_id = cavs.custom_attribute_id;
	
	INSERT INTO public.custom_attributes_values (
		custom_attribute_id,
		product_id,
		"value"
	)
	SELECT
		cavs.custom_attribute_id,
		cavs.product_id,
		cavs."value"
	FROM public.custom_attributes_values_staging AS cavs
	LEFT JOIN public.custom_attributes_values AS cav
	ON cav.product_id = cavs.product_id
		AND cav.custom_attribute_id = cavs.custom_attribute_id
	WHERE cav.product_id IS NULL;

	TRUNCATE public.custom_attributes_values_staging;
$BODY$;
ALTER PROCEDURE public.iu_sp_import_custom_attributes_values_staging()
    OWNER TO postgres;
