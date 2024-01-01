-- PROCEDURE: public.iu_sp_import_custom_attributes(text)

-- DROP PROCEDURE IF EXISTS public.iu_sp_import_custom_attributes(text);

CREATE OR REPLACE PROCEDURE public.iu_sp_import_custom_attributes(
	IN atts text)
LANGUAGE 'sql'
    SECURITY DEFINER 
AS $BODY$
	INSERT INTO public.custom_attributes (
		"name"
	)
	SELECT new_atts.text
	FROM STRING_TO_TABLE(atts, ',') AS new_atts
	LEFT JOIN public.custom_attributes
	ON custom_attributes."name" = new_atts.text
	WHERE custom_attributes."name" IS NULL
$BODY$;
ALTER PROCEDURE public.iu_sp_import_custom_attributes(text)
    OWNER TO postgres;
