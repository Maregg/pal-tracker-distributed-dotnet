cd PlatformServices
./gradlew EurekaServer:bootRun

cd ..
dotnet run --urls "http://*:8881" --project Applications/AllocationsServer
dotnet run --urls "http://*:8882" --project Applications/BacklogServer
dotnet run --urls "http://*:8883" --project Applications/RegistrationServer
dotnet run --urls "http://*:8884" --project Applications/TimesheetsServer