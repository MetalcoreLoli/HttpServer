# Simple http server

unfinished

## Сборка и запуск
```
dotnet cake 
```
после сборки появится директория *artifacts* внутри будет сам сервер.
вместе запуска сервера должен быть *index.html* файл его содержимое сервер будет отправлять

## Настройка сервера
настройка довольно простая и происходит в *config.json*

```{json}
{
    "Prefix": "http://localhost:1337/home/",
    "EndPoints": [
        {
            "Url":"test",
            "Page": "test.html"
        },
        {
            "Url":"help",
            "Page": "help.html"
        }
    ]
}
```
