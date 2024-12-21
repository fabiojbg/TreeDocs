# Introduction 
This project is a web application where users can create notes in a hierarchical way (tree). It is a small project I did some years ago to learn and teach Clean Architecture and related technologies as a practical project. It is not a finished project. I'm mainly a back-end developer, so the back-end code is fairly good, but the UI was written with one of the first versions of Blazor and needs to be rewritten. This is possible because the front-end is completely separated from the back-end, which uses the API-first approach and has all the necessary APIs for any new front-ends. That way, it should be relatively easy to rewrite the front-end without any changes to the back-end. The project uses MongoDB to store the user notes/docs and RedisCache as the external cache.

A running sample can be seen at this link: https://notes.botelho.cc/. (Note: This is for demonstration purposes and all changes made to this repository are reset every hour)

This project is good for learning the following technologies:

- Clean Architecture
- Domain Driven Design (DDD)
- API First
- JWT Authentication tokens
- MediaTR/CQRS
- Mongo DB
- Redis Cache
- Blazor-Client(Old version)
- Unit tests with xUnit

![alt text](./Images/Sample.png)

# Build and Test
The easiest way to execute this app is by using the Docker Compose files in the project's root directory. Just use the command **"docker compose up"** to build and run all project services and dependencies, like MongoDB, Redis, and Seq.

Once running, open your browser and type http://localhost:5555.

**Note**: This app uses SyncFusion Blazor components and you'll have to set your own SyncFusion license in the program.cs file of the TreeDocs.ClientApp project to be able to render the UI.

