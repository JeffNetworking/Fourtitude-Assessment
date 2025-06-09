
Reuqest

{
  "partnerkey": "FAKEGOOGLE",
  "partnerrefno": "FG-00001",
  "partnerpassword": "RkFLRVBBU1NXT1JEMTIzNA==",
  "totalamount": 100000,
  "items": [
    {
      "partneritemref": "i-00001",
      "name": "Pen",
      "qty": 4,
      "unitprice": 20000
    },
    {
      "partneritemref": "i-00002",
      "name": "Ruler",
      "qty": 2,
      "unitprice": 10000
    }
  ],
  "timestamp": "2025-06-09T02:27:09Z",
  "sig": "ZjYwNjcwMTkxOTIzM2NlOTc2MzYwZTY5YWRkMzA1YzZiNDRhZWEzZjc1MTVkYWU5OTk3NTk0MmJjZmMzY2FjNg=="
}


Response (Sucess)

{
  "result": 1,
  "totalAmount": 100000,
  "totalDiscount": 10000,
  "finalAmount": 90000,
  "resultMessage": null
}

Response (Fail)

{
  "result": 0,
  "totalAmount": null,
  "totalDiscount": null,
  "finalAmount": null,
  "resultMessage": "The total value in item details does not match the totalamount."
}


Noted : Signature and TimeStamp is different every 5 min. Ensure generate correct signature and timestamp



Check Log file in local
-Fourtitude-Assessment\Fourtitude-Assessment\bin\Debug\net6.0\log.txt

Dcoker command check the log.txt
-docker exec -it fourtitude-api-container  cat log.txt



use Docker build image
-docker build -t fourtitude-api .
-docker run -d -p 8081:80 --name fourtitude-api-container fourtitude-api



use http://localhost:8081/swagger to see the swagger.
