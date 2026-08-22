function groups = traverse(graph)
isVisit = false(size(graph,1),1);
groups = {};
for i = 1:size(graph,1)
    group = [];
    DFS(i);
    if ~isempty(group)
        groups{end+1} = group;
    end
end

    function DFS(index)
        if isVisit(index)
            return;
        end
        isVisit(index) = true;
        group(end+1) = index;

        connections = find(graph(index,:));
        for idx = 1:length(connections)
            DFS(connections(idx));
        end
    end
end