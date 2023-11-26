FROM public.ecr.aws/lambda/dotnet:6-arm64 as base

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build

WORKDIR /src
COPY Aws.CancellationTokens.AspNet.csproj /src
RUN dotnet restore
COPY . /src
RUN dotnet publish -c Release --runtime linux-arm64 --self-contained false -o /src/publish

FROM base AS final
WORKDIR /var/task
COPY --from=build /src/publish .

CMD ["Aws.CancellationTokens.AspNet::Aws.CancellationTokens.AspNet.LambdaEntryPoint::FunctionHandlerAsync"]
