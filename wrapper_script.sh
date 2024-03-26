#!/bin/bash

# turn on bash's job control
#set -m

nohup dotnet ./Users/Users.API.dll > Users.log &&
nohup dotnet ./Basket/Basket.API.dll > Basket.log &&
nohup dotnet ./Catalog/Catalog.API.dll > Catalog.log &&
nohup dotnet ./Client/eShopping.Client.dll > Client.log

#dotnet ./Users/Users.API.dll &
#dotnet ./Basket/Basket.API.dll &
#dotnet ./Catalog/Catalog.API.dll &
#dotnet ./Client/eShopping.Client.dll

# now we bring the primary process back into the foreground
# and leave it there
#fg %1