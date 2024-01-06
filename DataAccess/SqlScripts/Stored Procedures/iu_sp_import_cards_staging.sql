-- PROCEDURE: public.iu_sp_import_cards_staging()

-- DROP PROCEDURE IF EXISTS public.iu_sp_import_cards_staging();

CREATE OR REPLACE PROCEDURE public.iu_sp_import_cards_staging(
	)
LANGUAGE 'sql'
	SECURITY DEFINER
AS $BODY$
	UPDATE public.cards 
	SET 
		shipping_category_id = cards_staging.shipping_category_id,
		duplicate = cards_staging.duplicate,
		product_url_name = cards_staging.product_url_name,
		product_type_id = cards_staging.product_type_id,
		rarity_name = cards_staging.rarity_name,
		sealed = cards_staging.sealed,
		market_price = cards_staging.market_price,
		lowest_price_with_shipping = cards_staging.lowest_price_with_shipping,
		product_name = cards_staging.product_name,
		set_id = cards_staging.set_id,
		score = cards_staging.score,
		set_name = cards_staging.set_name,
		foil_only = cards_staging.foil_only,
		set_url_name = cards_staging.set_url_name,
		seller_listable = cards_staging.seller_listable,
		total_listings = cards_staging.total_listings,
		product_line_id = cards_staging.product_line_id,
		product_status_id = cards_staging.product_status_id,
		max_fulfullable_quantity = cards_staging.max_fulfullable_quantity,
		lowest_price = cards_staging.lowest_price
	FROM public.cards_staging
	WHERE cards.product_id = cards_staging.product_id;
	
	INSERT INTO public.cards (
		shipping_category_id,
		duplicate,
		product_url_name,
		product_type_id,
		rarity_name,
		sealed,
		market_price,
		lowest_price_with_shipping,
		product_name,
		set_id,
		product_id,
		score,
		set_name,
		foil_only,
		set_url_name,
		seller_listable,
		total_listings,
		product_line_id,
		product_status_id,
		max_fulfullable_quantity,
		lowest_price
	)
	SELECT
		cards_staging.shipping_category_id,
		cards_staging.duplicate,
		cards_staging.product_url_name,
		cards_staging.product_type_id,
		cards_staging.rarity_name,
		cards_staging.sealed,
		cards_staging.market_price,
		cards_staging.lowest_price_with_shipping,
		cards_staging.product_name,
		cards_staging.set_id,
		cards_staging.product_id,
		cards_staging.score,
		cards_staging.set_name,
		cards_staging.foil_only,
		cards_staging.set_url_name,
		cards_staging.seller_listable,
		cards_staging.total_listings,
		cards_staging.product_line_id,
		cards_staging.product_status_id,
		cards_staging.max_fulfullable_quantity,
		cards_staging.lowest_price
	FROM public.cards_staging
	LEFT JOIN public.cards
		ON cards.product_id = cards_staging.product_id
	WHERE cards.product_id IS NULL;
	
	TRUNCATE TABLE public.cards_staging;
$BODY$;
ALTER PROCEDURE public.iu_sp_import_cards_staging()
    OWNER TO postgres;
