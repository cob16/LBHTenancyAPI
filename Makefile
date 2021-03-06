TEST_UH_CONNECTION_STRING="Data Source=tcp:stubuniversalhousing;Initial Catalog=StubUH;Integrated Security=False;User ID=sa;Password=Rooty-Tooty"

.PHONY: docker-build
docker-build:
	docker-compose build --build-arg VERSION_SUFFIX='local-build'

.PHONY: docker-down
docker-down:
	docker-compose down

.PHONY: setup
setup: docker-build

.PHONY: serve
serve:
	docker-compose run -p 3000:80 -e UH_URL=$(TEST_UH_CONNECTION_STRING) --rm lbhtenancyapi

.PHONY: test
test:
	docker-compose run -e UH_CONNECTION_STRING=$(TEST_UH_CONNECTION_STRING) --rm lbhtenancyapitest dotnet test

.PHONY: shell
shell:
	docker-compose run --rm lbhtenancyapi /bin/bash

.PHONY: check
check: lint test
	echo 'Deployable!'
