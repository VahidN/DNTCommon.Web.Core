"C:\Program Files\Git\usr\bin\openssl.exe" genrsa -out private.pem 2048  
"C:\Program Files\Git\usr\bin\openssl.exe" rsa -in private.pem -pubout -out public.pem
"C:\Program Files\Git\usr\bin\openssl.exe" req -new -x509 -key private.pem -out certificate.pem -days 36500 -subj "/CN=DNT" 
"C:\Program Files\Git\usr\bin\openssl.exe" pkcs12 -export -in certificate.pem -inkey private.pem -out cert123.pfx -name "ExampleCert" -CApath . -certfile certificate.pem -passout pass:123
pause


