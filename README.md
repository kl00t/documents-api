# Documents

## Setup

1) Create a bucket with non public access in AWS. Replace name in settings 'BucketName' 
2) Configure permissions for your user to access buckets in your account
3) Generate an Access Key for your user.
4) Replace your AWS 'AccessKey' and 'SecretKey' in user secrets (right click 'Manage User Secrets' in Visual Studio)

###GET /customers/{customerId}/documents?documentType={documentType}
###GET /customers/{customerId}/documents/{documentId}
###DELETE /customers/{customerId}/documents/{documentId}
###POST /customers