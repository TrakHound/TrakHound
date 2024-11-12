WITH RECURSIVE trakhound_object_child_tree(requested_id, uuid, parent_uuid, level) AS (
	select
	[a].[uuid] as [requested_id],
	[a].[uuid], 
	[a].[parent_uuid],
	1 as [level]
	from [trakhound_objects] as [a]
	where [a].[uuid] in (select [id] from _parentUuids)

	union all

	select
	[c].[requested_id], 
	[b].[uuid], 
	[b].[parent_uuid],
	[c].[level] + 1 as [level]
	from [trakhound_objects] as [b]
	join trakhound_object_child_tree as [c] on [b].[parent_uuid] = [c].[uuid] 
 )
select [requested_id], [uuid], [level] from trakhound_object_child_tree