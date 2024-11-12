drop table if exists _objectData;
create temp table _objectData ([requested_id] text, [uuid] text, [version] text, [parent_uuid] text, [name] text, [parent_name] text, [level] integer, [path] text);

with recursive cte([requested_id], [uuid], [version], [parent_uuid], [name], [parent_name], [level], [path]) as
(
	select
	[targets].[uuid] as [requested_id],
	[targets].[uuid],
	[targets].[version],
	[targets].[parent_uuid],
	[targets].[name],
	[target_parents].[name] as [parent_name],
	0 as [level],
	IIF([target_parents].[name] is null, [targets].[name], [target_parents].[name] || '/' || [targets].[name]) as [path]
	from [trakhound_objects] as [targets]
	left join [trakhound_objects] as [target_parents] on [targets].[parent_uuid] = [target_parents].[uuid]
	where [targets].[uuid] in (select [id] from _uuids)

	union all

	select
	[c].[requested_id],
	[b].[uuid], 
	[b].[version],
	[b].[parent_uuid],
	[b].[name],
	[d].[name] as[parent_name],
	[c].[level] + 1 as [level],
	[d].[name] || '/' || [c].[path] as [path]
	from [trakhound_objects] as [b]
	join cte as [c] on [b].[uuid] = [c].[parent_uuid]  
	join [trakhound_objects] as [d] on [b].[parent_uuid] = [d].[uuid]
)
insert into _objectData select [requested_id], [uuid], [version], [parent_uuid], [name], [parent_name], [level], [path] from cte;


select
lower([path]) as [path],
[a].[version] as [version],
[a].[requested_id] as [uuid]
from _objectData [a]
inner join (select [requested_id], max([level]) as [level] from _objectData group by [requested_id]) as [c] on [a].[requested_id] = [c].[requested_id] and [a].[level] = [c].[level]
