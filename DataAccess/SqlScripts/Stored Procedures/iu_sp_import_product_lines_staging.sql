-- PROCEDURE: public.iu_sp_import_product_lines_staging()

-- DROP PROCEDURE IF EXISTS public.iu_sp_import_product_lines_staging();

CREATE OR REPLACE PROCEDURE public.iu_sp_import_product_lines_staging(
	)
LANGUAGE 'sql'
    SECURITY DEFINER 
AS $BODY$
	UPDATE public.product_lines AS pl
		SET product_line_name = pls.product_line_name,
			product_line_url_name = pls.product_line_url_name
	FROM public.product_lines_staging AS pls
	WHERE pl.product_line_id = pls.product_line_id;
	
	INSERT INTO public.product_lines (
		product_line_id,
		product_line_name,
		product_line_url_name
	)
	SELECT
		pls.product_line_id,
		pls.product_line_name,
		pls.product_line_url_name
	FROM public.product_lines_staging AS pls
	LEFT JOIN public.product_lines AS pl
	ON pl.product_line_id = pls.product_line_id
	WHERE pl.product_line_id IS NULL;

	TRUNCATE public.product_lines_staging;
$BODY$;
ALTER PROCEDURE public.iu_sp_import_product_lines_staging()
    OWNER TO postgres;
