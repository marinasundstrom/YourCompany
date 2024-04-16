openssl genrsa -aes256 -passout pass:Abc123! -out certs/server.pass.key 4096

cd certs

openssl rsa -passin pass:Abc123! -in server.pass.key -out server.key
rm server.pass.key
openssl req -new -key server.key -out server.csr -config <(cat server.cnf)

openssl x509 -req -extensions v3_req -sha256 -days 365 -in server.csr -signkey server.key -out server.crt -extfile server.cnf

sudo security add-trusted-server -d -r trustRoot -k /Library/Keychains/System.keychain server.crt

cd ..