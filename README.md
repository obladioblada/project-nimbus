# project-nimbus

I decided to focus on the implementation of the file upload to s3 (by using a presigned URL obtained by calling a lambda function,
which would be behind Api Gateway) and the dynamoDb metadata insertion by leveraging the S3 event notification feature, 
which triggers the lambda function in charge of updating the DynamoDb Table.  
Because of time, the system is missing path handling and verification.
To simulate the AWS cloud environment I used localstack so to be able to have a working example which would run locally.
Used CDK for IaC to spin up the required resources.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Deploying with CDKLocal](#deploying-with-cdklocal)
- [Running the Application](#running-the-application)

## Prerequisites

1. [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
2. [AWS CLI](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html)
3. [Node.js](https://nodejs.org/) (for CDK)
4. [AWS CDK CLI](https://docs.aws.amazon.com/cdk/latest/guide/cli.html)
5. [Docker](https://docs.docker.com/get-docker/) (for LocalStack)
6. [LocalStack](https://localstack.cloud/) (installed via Docker compose)
7. [cdklocal](https://github.com/localstack/aws-cdk-local) CLI wrapper

## Project Structure

```
├───cdk
│   ├───cdk.out
│   ├───lib
│   ├───MetadataHandlerLambda
│   │   ├───src
│   │   │   └───MetadataHandlerLambda
│   │   └───test
│   │       └───MetadataHandlerLambda.Tests
│   │           
│   ├───src
│   │   └───Cdk
│   │      
│   └───UploaderHandlerLambda
│       └───src
│           └───UploaderHandlerLambda
├───nimbus.html
├───response.json
├───docker-compose.yml
```


## Getting Started

1. **Install CDK and CDKLocal:**

```shell
npm install -g aws-cdk
npm install -g aws-cdk-local
```

2. **Run localstack**

```shell
docker compose up
```

## Deploying with CDKLocal

```shell
cd ./cdk/
cdklocal bootstrap
cdklocal synth
cdklocal deploy
```

> [!IMPORTANT]  
> The below snippet has to be run to bypass [localstack bug](https://stackoverflow.com/questions/78311472/aws-cdk-create-s3-event-notification-to-sqs-message-in-localstack) which doesn't properly create the s3 event notification using CDK. 

```shell
aws --endpoint-url=http://localhost:4566 s3api put-bucket-notification-configuration --bucket storage-test --notification-configuration '{ \"LambdaFunctionConfigurations\": [ { \"Id\": \"file-upload-notification\", \"LambdaFunctionArn\": \"arn:aws:lambda:us-east-1:000000000000:function:metadata-handler-test\", \"Events\": [ \"s3:ObjectCreated:*\"] } ] }'                                                                                                                                                                      
```
To check that the operation went well, run the below snippet. In case of success you should get the ***LambdaFunctionConfigurations*** in response.

```shell
aws --endpoint-url=http://localhost:4566 s3api get-bucket-notification-configuration --bucket storage-test 
```
## Running the Application (Simulate client)

### Get Presigned URL

In order to get the presigned URL to use to upload a file ( which should be a client application task) use the below.
From the project root directory run:

```shell
aws --endpoint-url=http://localhost:4566 lambda invoke --function-name uploader-handler-test  --cli-binary-format raw-in-base64-out  --payload '{ \"FileName\": \"file.txt\" }' response.json
```
The lambda responses returning the presigned URL and saves it in the [response.json](response.json) file.

> [!WARNING]  
> Pay attention to the double quotes wrapping the URL we need to use in the next step.

### Upload file

In order to upload the file, open [nimbus.html](nimbus.html) in a browser. Paste the presigned url you got from invoking the ***uploader-handler-test*** lambda function and select the file you want to upload. 

### Verify Metadata Table
Invoking the uploader imitates an API call via Api Gateway. For lack of time instead, we invoke the lambda by using the AWS CLI with the command below.
If all went fine, the table should contain a raw with the metadata of the uploaded file. The below snippet scans the table and returns its content.

```shell
aws --endpoint-url=http://localhost:4566 dynamodb scan --table-name metadata-table-test                                                                                                           
```

# Unit Tests

> [!NOTE]  
> Testing if not where close to be comprehensive. I decided to create some unit tests for the MetadataHandlerLambda lambda to show how I normally use to set them up.
### MetadataHandlerLambda Project.

#### Execute unit tests
```shell
    cd "cdk/MetadataHandlerLambda/test/MetadataHandlerLambda.Tests"
    dotnet test
```

> [!NOTE]  
> Working with:  
> - Docker version 27.3.1, build ce12230  
> - .NET SDK version 8.0.404  
> - cdk version 2.166.0 (build 7bb9203)   
> - aws-cli/2.15.52 Python/3.11.8 Windows/10 exe/AMD64
