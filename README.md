# project-nimbus

to install
 - aws cli 
 - localstack(?)
 - cdklocal

# Setting up the infrastructure
### Docker

```shell
docker compose up
```

### CDK

```shell
cd ./cdk/
```

```shell
cdklocal bootstrap
```

```shell
cdklocal synth
```

```shell
cdklocal deploy
```
!!! This has to be run to bypass localstack bug which doesn't properly create the event notification using CDK. 
```shell
aws --endpoint-url=http://localhost:4566 s3api put-bucket-notification-configuration --bucket storage-test --notification-configuration '{ \"LambdaFunctionConfigurations\": [ { \"Id\": \"file-upload-notification\", \"LambdaFunctionArn\": \"arn:aws:lambda:us-east-1:000000000000:function:metadata-handler-test\", \"Events\": [ \"s3:ObjectCreated:*\"] } ] }'
aws --endpoint-url=http://localhost:4566 s3api get-bucket-notification-configuration --bucket storage-test                                                                                                                                                                       
```

### Get Presigned URL

```shell
aws --endpoint-url=http://localhost:4566 lambda invoke --function-name uploader-handler-test  --cli-binary-format raw-in-base64-out  --payload '{ \"FileName\": \"file.txt\" }' response.json
```


### Upload file

In order to upload the file, use the client.html under the root directory. Paste the presigned url you got from invoking the uploader-handler-test lambda function and select the file you want to upload. 

### Verify Metadata Table

```shell
aws --endpoint-url=http://localhost:4566 dynamodb scan --table-name metadata-table-test                                                                                                           
```
Invoking the uploader imitates an API call via Api Gateway. For lack of time instead, we invoke the lambda by using the AWS CLI with the command above.  
The lambda responses returning the presigned URL stored in the response.json file 

aws --endpoint-url=http://localhost:4566  dynamodb list-tables

