```
dotnet dev-certs https -ep aspnetapp.pfx -p crypticpassword
dotnet dev-certs https --trust
```

```
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain <<certificate>>'
```

Extract private key

```
openssl pkcs12 -in aspnetapp.pfx -nocerts -out localhost.key
```


Extract certificate

```
openssl pkcs12 -in aspnetapp.pfx -clcerts -nokeys -out localhost.crt
```

Remove passphrase from key

```
cp localhost.key localhost.key.bak
openssl rsa -in localhost.key.bak -out localhost.key
```