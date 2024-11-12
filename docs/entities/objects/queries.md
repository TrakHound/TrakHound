# Queries
> [!Important]
> Queries are still under development

Queries are used to query data within TrakHound Entities using a SQL similar language.

## Example 1:
```
select * from [/shopfloor/machines/*];
```

## Example 2:
```
select > from [/shopfloor/machines/*];
```

## Example 3:
```
select >> from [/shopfloor/machines/*];
```

## Example 4:
```
select [name] from [/shopfloor/machines/*];
```

## Example 5:
```
select * from [/shopfloor/machines/*] where [name] = 'Machine #1';
```
