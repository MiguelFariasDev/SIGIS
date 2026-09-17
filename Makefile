.PHONY: help up down logs build migrate seed test test-back test-front test-mobile demo clean fmt

help: ## Mostra esta ajuda
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-12s\033[0m %s\n", $$1, $$2}'

up: ## docker compose up -d
	docker compose up -d

down: ## docker compose down
	docker compose down

logs: ## docker compose logs -f
	docker compose logs -f

build: ## docker compose build
	docker compose build

migrate: ## Roda migrations do EF Core
	dotnet ef database update \
		--project sigis-backend/src/Sigis.Infrastructure \
		--startup-project sigis-backend/src/Sigis.Api

seed: ## Popula dados de demonstração
	dotnet run --project sigis-backend/src/Sigis.Api -- --seed

test: test-back test-front test-mobile ## Roda dotnet test + npm test + flutter test

test-back: ## Só backend
	dotnet test sigis-backend/Sigis.sln

test-front: ## Só frontend
	cd sigis-frontend && npm test --if-present

test-mobile: ## Só mobile
	cd sigis-mobile && flutter test

demo: seed ## seed + abre o front
	xdg-open http://localhost:5173 || open http://localhost:5173

clean: ## down -v + limpa bin/obj + node_modules
	docker compose down -v
	find sigis-backend -type d \( -name bin -o -name obj \) -exec rm -rf {} +
	rm -rf sigis-frontend/node_modules sigis-frontend/dist
	rm -rf sigis-mobile/build

fmt: ## dotnet format
	dotnet format sigis-backend/Sigis.sln
