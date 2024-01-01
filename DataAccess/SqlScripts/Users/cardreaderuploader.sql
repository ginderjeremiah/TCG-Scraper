-- Role: card_reader_uploader
DROP ROLE IF EXISTS cardreaderuploader;
DROP ROLE IF EXISTS card_reader_uploader;

CREATE ROLE card_reader_uploader WITH
  LOGIN
  NOSUPERUSER
  INHERIT
  NOCREATEDB
  NOCREATEROLE
  NOREPLICATION
  ENCRYPTED PASSWORD 'SCRAM-SHA-256$4096:hGnwsZBQu70HAIBHfygEtg==$muABqoesf6Tlc+QjZE5hjYhpkRDR2nXmOnvYQKDwV7c=:CGFbr4cVHJwsWZsvniYAHd3jnYOBLg+Cr16gxwGG0u4=';

GRANT pg_read_all_data TO card_reader_uploader;