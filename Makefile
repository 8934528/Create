.PHONY: all install run-ai run-backend run-frontend run-all clean db-update help

# Variables
PYTHON_VENV = ai-service/venv
PYTHON_BIN = $(PYTHON_VENV)/Scripts/python
AI_SERVICE_DIR = ai-service/app
BACKEND_DIR = backend-dotnet/Create.API
FRONTEND_DIR = frontend-web

help:
	@echo "========================================================================"
	@echo "CREATE PROJECT MANAGEMENT"
	@echo "========================================================================"
	@echo "  Setup & Database:"
	@echo "    make install          - Setup virtual environments, restore packages, and install tools"
	@echo "    make setup-tools      - Install/Update global .NET tools (dotnet-ef)"
	@echo "    make db-update        - Apply database migrations via EF Core"
	@echo ""
	@echo "  Run Services:"
	@echo "    make run-ai           - Start Python AI Service"
	@echo "    make run-backend      - Start .NET Backend API"
	@echo "    make run-frontend     - Start .NET Frontend Web"
	@echo ""
	@echo "  Development (Watch Mode):"
	@echo "    make watch-backend    - Start Backend with 'dotnet watch'"
	@echo "    make watch-frontend   - Start Frontend with 'dotnet watch'"
	@echo ""
	@echo "  Global Commands:"
	@echo "    make run-all          - Start all services concurrently"
	@echo "    make stop-all         - Stop all running dotnet/python processes"
	@echo "    make clean            - Remove build artifacts and temporary files"
	@echo "========================================================================"

install: setup-tools
	@echo ">>> Setting up Python virtual environment..."
	cd ai-service && python -m venv venv
	$(PYTHON_BIN) -m pip install --upgrade pip
	$(PYTHON_BIN) -m pip install -r ai-service/requirements.txt
	@if [ ! -f ai-service/.env ]; then echo "!!! Warning: ai-service/.env not found !!!"; fi
	@echo ">>> Restoring .NET Backend packages..."
	dotnet restore backend-dotnet/Create.sln
	@echo ">>> Restoring .NET Frontend packages..."
	dotnet restore $(FRONTEND_DIR)/frontend-web.csproj
	@if [ ! -f .env ]; then echo "!!! Warning: Root .env not found !!!"; fi

setup-tools:
	@echo ">>> Ensuring .NET EF tools are installed..."
	-dotnet tool install --global dotnet-ef || dotnet tool update --global dotnet-ef

db-update:
	@echo ">>> Updating database..."
	cd $(BACKEND_DIR) && dotnet ef database update

run-ai:
	@echo ">>> Starting AI Service..."
	cd $(AI_SERVICE_DIR) && ../venv/Scripts/python main.py

run-backend:
	@echo ">>> Starting Backend API..."
	cd $(BACKEND_DIR) && dotnet run

run-frontend:
	@echo ">>> Starting Frontend Web..."
	cd $(FRONTEND_DIR) && dotnet run

watch-backend:
	@echo ">>> Starting Backend API (Watch Mode)..."
	cd $(BACKEND_DIR) && dotnet watch run

watch-frontend:
	@echo ">>> Starting Frontend Web (Watch Mode)..."
	cd $(FRONTEND_DIR) && dotnet watch run

run-all:
	@echo ">>> Starting all services concurrently..."
	@echo "Note: This will run AI, Backend, and Frontend in the background."
	@$(MAKE) run-ai & $(MAKE) run-backend & $(MAKE) run-frontend

stop-all:
	@echo ">>> Stopping all running services..."
	-taskkill /F /IM dotnet.exe /T
	-taskkill /F /IM python.exe /T

clean:
	@echo ">>> Cleaning up..."
	-rm -rf ai-service/venv
	-find . -type d -name "bin" -exec rm -rf {} +
	-find . -type d -name "obj" -exec rm -rf {} +
	@echo "Done."
