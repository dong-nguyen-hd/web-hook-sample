# Web Hook Project:

This project is built on **.NET 8**, and is used to create lightweight webhooks.

## Technologies

* [ASP.NET Core API 8](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-8.0)
* [Entity Framework Core 8](https://docs.microsoft.com/en-us/ef/core/)
* [Hangfire](https://www.hangfire.io/)
* [PostgreSQL](https://www.postgresql.org/)
* [Redis](https://redis.io/)

## About
Classify tasks based on execution time into categories such as "long-term", "soon", and "immediate" and allocate storage accordingly to each type of database. "Long-term" tasks are pushed to PostgreSQL, "soon" tasks are sent to Hangfire-Redis, while "immediate" tasks are prioritized for instant execution.

This is a lightweight application that supports deployment with "Horizontal Pod Autoscaling", enabling webhooks through RESTful API and allowing task status lookup.

## License

This project is licensed with the [MIT license](LICENSE).