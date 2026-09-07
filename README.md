# MiniCommerce

A small sample e-commerce application written in C# (.NET), created for learning and prototyping purposes.

## Overview

MiniCommerce is a simple online store that provides basic features such as product management, a shopping cart, order placement, and a simulated payment flow. This repository is intended for learning layered architecture, building APIs with ASP.NET Core, and getting familiar with the .NET development workflow.

## Features

- Product management (CRUD)
- Simple shopping cart
- Order creation
- Clear, extensible project structure
- Includes a Dockerfile for containerized runs

## Technologies

- Language: C#
- Framework: .NET 6/7 (check the project files for the exact target framework)
- Container: Docker (Dockerfile present in the repository)

## Prerequisites

- .NET SDK (recommended: 6 or 7)
- Docker (optional, for container runs)

## Local setup and run

1. Clone the repository:

   git clone https://github.com/SinaSabet/MiniCommerce.git
   cd MiniCommerce

2. Restore packages:

   dotnet restore

3. Build and run:

   dotnet build
   dotnet run --project ./src/YourProjectName/YourProjectName.csproj

Replace `./src/YourProjectName/YourProjectName.csproj` with the actual path to the main .csproj file in this repository.

## Running with Docker

1. Build the image:

   docker build -t minicommerce:latest .

2. Run the container:

   docker run -p 5000:80 minicommerce:latest

The internal container port may differ; check the Dockerfile and application settings if needed.

## Tests

If there are unit tests included, run them with:

   dotnet test

## Project structure

- /src: application source code
- /tests: unit and integration tests (if present)
- Dockerfile: container build definition

If you'd like, I can add a more detailed tree of files and explain each module.

## Contributing

Contributions are welcome.

1. Open an issue to discuss major changes.
2. Create a branch for your work: `git checkout -b feature/your-feature`.
3. Commit and push your changes, then open a Pull Request.

## License

If you want this project to be open-source, add a LICENSE file (for example MIT) to the repository. Currently no license file is included.

## Contact

- Repository owner: SinaSabet
- Repository: https://github.com/SinaSabet/MiniCommerce

---

Notes:
- I updated README.md to an English version.
- I cannot pin the repository to your GitHub profile automatically, but you can do it from your profile page: go to your profile → "Customize your pins" → select MiniCommerce → Save.

Next steps I can do for you (pick any):
- Add badges (build, coverage, .NET) to the README
- Add more detailed setup instructions with the exact .csproj path and environment variables
- Add API examples (sample requests and responses) and screenshots
- Create a simple GitHub Actions CI workflow and add its badge

Would you like me to apply any of these changes now?