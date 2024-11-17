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
!!! This has to be run to bypass localstack bug
```shell
aws --endpoint-url=http://localhost:4566 s3api put-bucket-notification-configuration --bucket storage-test --notification-configuration '{ \"LambdaFunctionConfigurations\": [ { \"Id\": \"file-upload-notification\", \"LambdaFunctionArn\": \"arn:aws:lambda:us-east-1:000000000000:function:metadata-handler-test\", \"Events\": [ \"s3:ObjectCreated:*\"] } ] }'
aws --endpoint-url=http://localhost:4566 s3api get-bucket-notification-configuration --bucket storage-test                                                                                                                                                                       
```

```shell
aws --endpoint-url=http://localhost:4566 lambda invoke --function-name uploader-handler-test  --cli-binary-format raw-in-base64-out  --payload '{ \"FileName\": \"file.txt\" }' response.json
```
    
Verify Lambdas have been published
aws --endpoint-url=http://localhost:4566 lambda list-functions
aws lambda get-function-configuration --function-name  ProjectNimbusStack-metadatahandlertestE4FD1-389717bf --endpoint-url=http://localhost:4566


### Get Presigned URL

```shell

```

### Upload file

```shell

```

### Verify Metadata Table

```shell
 aws --endpoint-url=http://localhost:4566 dynamodb scan --table-name <dynamodb-table-name>
```


aws --endpoint-url=http://localhost:4566  dynamodb list-tables

