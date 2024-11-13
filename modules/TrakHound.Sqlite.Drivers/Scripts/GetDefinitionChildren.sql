WITH RECURSIVE trakhound_definition_child_tree(requested_id, uuid, parent_uuid, level) AS (
	select
	[a].[uuid] as [requested_id],
	[a].[uuid], 
	[a].[parent_uuid],
	0 as [level]
	from [trakhound_definitions] as [a]
	where [a].[uuid] in (select [id] from _uuids)

	union all

	select
	[c].[requested_id], 
	[b].[uuid], 
	[b].[parent_uuid],
	[c].[level] + 1 as [level]
	from [trakhound_definitions] as [b]
	join trakhound_definition_child_tree as [c] on [b].[parent_uuid] = [c].[uuid] 
 )
select [requested_id], [uuid], 1 as [type_id] from trakhound_definition_child_tree where [level] > 0;
