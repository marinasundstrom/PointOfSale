# PointOfSale
 
A Point of Sale app - demonstrating a distributed system supported by a well-crafted UI.

It is a proof of concept. Check the Disclaimer below.

## Videos

* [General checkout UI](https://www.youtube.com/watch?v=wDP1ShXDgY8)
* [Auto-apply discount to item](https://www.youtube.com/watch?v=5MCQMhSh_gg)
* [Hypermedia API](https://www.youtube.com/watch?v=LbCUFEter_A)

## Background

This is an attempt at learning micro-services and asyncronous messaging that started as simply a "Point of Sale" app.

Due to its nature, learn as you go, this is not the most complete sample.

### The Purpose

To learn and share knowledge on how to design and implement a complex project based on [micro] services. And to have fun, at the same time. :) 

Feel free to take inspiration from this work of art!

### Development approach

It started when crafting a UI and adding the basic functionality. Thinking about the domain, what a User would expect from an app such as this - a general Behavior-centric approach to development.

First there was the Product Catalog, and then came the Cart system. Later the Customers service was added. Then the ability to create Receipts.

### Project structure

The distributed app consists of code in .NET 6, using ASP.NET Core, and Blazor for UI. 

As a development orchestrator, Tye is used. It has some Docker image dependencies that are also being managed by Tye.

SQL Server, Nginx, RabbitMQ, and Redis, are all running in Docker. Check ```tye.yaml```.

The main services are: Carts, Catalog, Customers, Billing. The supporting project is Marketing which house the Discounts. The rest are in a basic state, with some not interacting with the main app.

## Disclaimer

This is an educational effort that was created for fun when trying to understand the domain and how to implement it.
It has never been made with any intention to be set into production.

It SHOULD NOT be used in production.

## How to run

First, make sure that .NET 6 SDK and the Tye global tools are installed.
You also need Docker.

Then, in the solution root directory, run:
```
tye run
```

This will start all the services and Docker containers.

Dashboard:
```
http://localhost:8000
```

Point of Sales UI:
```
https://localhost:8080/
```

You have to configure the certificates for SSL to work with Nginx. Check ```cert.md```.
