with recursive trakhound_object_parent_tree(requested_id, uuid, parent_uuid, level) as
(
	select
	[a].[uuid] as [requested_id],
	[a].[uuid],
	[a].[parent_uuid],
	0 as [level]
	from [trakhound_objects] as [a]
	where [a].[uuid] in (select [id] from _uuids)

	union all

	select
	[c].[requested_id],
	[b].[uuid], 
	[b].[parent_uuid],
	[c].[level] - 1 as [level]
	from [trakhound_objects] as [b]
	join trakhound_object_parent_tree as [c] on [b].[uuid] = [c].[parent_uuid]  
)
select [requested_id], [uuid], -1 as [type_id] from trakhound_object_parent_tree where [level] < 0
union
select [id] as [requested_id], [id] as [uuid], 0 as [type_id] from _uuids;