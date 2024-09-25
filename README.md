# Documents

GET /customers/{customerId}/documents?documentType={documentType}

POST /customers/{customerId}/documents
{
  "fileName": "string",
  "contentType": "string",
  "contentBody": "string"
}

GET /customers/{customerId}/documents/orders/{orderCode}?documentType={documentType}

POST /customers/{customerId}/documents/orders/{orderCode}
{
  "fileName": "string",
  "contentType": "string",
  "contentBody": "string"
}