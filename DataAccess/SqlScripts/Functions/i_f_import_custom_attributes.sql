-- FUNCTION: public.i_f_import_custom_attributes(text)

-- DROP FUNCTION IF EXISTS public.i_f_import_custom_attributes(text);

CREATE OR REPLACE FUNCTION public.i_f_import_custom_attributes(
	atts text)
    RETURNS SETOF custom_attributes 
    LANGUAGE 'sql'
    COST 100
    VOLATILE SECURITY DEFINER PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
	INSERT INTO public.custom_attributes (
		"name"
	)
	SELECT new_atts.text
	FROM STRING_TO_TABLE(atts, ',') AS new_atts
	LEFT JOIN public.custom_attributes
	ON custom_attributes."name" = new_atts.text
	WHERE custom_attributes."name" IS NULL;
	
	SELECT * FROM public.custom_attributes
$BODY$;

ALTER FUNCTION public.i_f_import_custom_attributes(text)
    OWNER TO postgres;
