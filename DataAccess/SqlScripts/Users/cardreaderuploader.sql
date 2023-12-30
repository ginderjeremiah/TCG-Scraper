-- Role: cardreaderuploader
-- DROP ROLE IF EXISTS cardreaderuploader;

CREATE ROLE cardreaderuploader WITH
  LOGIN
  NOSUPERUSER
  INHERIT
  NOCREATEDB
  NOCREATEROLE
  NOREPLICATION
  ENCRYPTED PASSWORD 'SCRAM-SHA-256$4096:hGnwsZBQu70HAIBHfygEtg==$muABqoesf6Tlc+QjZE5hjYhpkRDR2nXmOnvYQKDwV7c=:CGFbr4cVHJwsWZsvniYAHd3jnYOBLg+Cr16gxwGG0u4=';

GRANT pg_read_all_data, pg_write_all_data TO cardreaderuploader;