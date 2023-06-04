# Introduction 
This project is a web application where users can create notes in a hierarchical way (tree). It is a little project I have done some years ago to learn and teach Clean Architecture and correlating technologies as a practical project. 
It is not a finished project. I'm mainly a back-end developer so the back-end code is fairly good but the UI was written with one of the first versions of Blazor and needs to be rewritten. The intent is to rewrite it using better UI technologies. This is possible because the front-end is completely separated from the back-end that uses the API-first approach which has all necessary APIs for any new front-ends. That way, should be relatively easy to re-write the front-end without any change to the back-end. The project uses MongoDB to store the user notes/docs and RedisCache as the external cache.

An running sample can be seen in this link https://notes.botelho.cc/. (Note: All changes made to this repository is reset every hour)

This project is good to learn the following technologies:

- Clean Architecture
- Domain Driven Design (DDD)
- API First
- JWT Authentication tokens
- MediaTR/CQRS
- Mongo DB
- Redis
- Blazor-Client(Old version)

![alt text](./Images/Sample.png)

# Build and Test
The easiest way to execute this app is using the docker compose files in the project root directory. Just use the command **"docker compose up"** to build and run all project services and dependencies like mongodb, redis and seq.

Once running, open your browser and type http://localhost:5555.

**Note**: this app uses SyncFusion Blazor components and you'll have to set your own SyncFusion license in the program.cs file of the TreeDocs.ClientApp project to be able to render the UI.

