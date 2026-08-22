extension = ".gif";
files = dir("*" + extension);
for fileID = 1:length(files)
    file = files(fileID);
    [~,filename,~] = fileparts(file.name);
    filename = string(filename);

    im = imread(filename + extension);

    if exist(filename + " textbox.mat", "file")
        load(filename + " textbox.mat");
    else
        textbox = detectTextCRAFT(im,CharacterThreshold=0.3); % time consuming, cache?
        textbox = merge(textbox);
        save(filename + " textbox.mat", 'textbox');
    end

    clf
    imshow(im);
    showShape('rectangle', textbox);

    gray = im2gray(im);
    binary = imbinarize(gray);

    vertical = [];
    for i = 1:size(textbox,1)
        if textbox(i,4) > textbox(i,3)
            vertical(end+1) = i;
        end
    end
    horizontal = 1:size(textbox,1);
    horizontal(vertical) = [];

    outputH = ocr(binary, textbox(horizontal,:), "LayoutAnalysis", "word");
    recognizedText = cat(1, {outputH(:).TextLines});
    for i = length(recognizedText):-1:1
        if isempty(recognizedText{i})
            recognizedText(i) = [];
            horizontal(i) = [];
        end
    end
    imshow(im);
    showShape("rectangle",textbox(horizontal,:),Label=recognizedText,Color="yellow")
    hold on

    for i = 1:length(vertical)
        roi = imcrop(binary, textbox(vertical(i),:));
        outputV = ocr(rot90(roi,3), "LayoutAnalysis", "word");
        showShape("rectangle",textbox(vertical(i),:),Label=outputV.Text,Color="yellow")
    end
end

function textbox = merge(textbox)
if size(textbox,1) < 2
    return
end

for i = 1:size(textbox,1)
    for j = i+1:size(textbox,1)
        x1 = textbox(i,1);
        y1 = textbox(i,2);
        w1 = textbox(i,3);
        h1 = textbox(i,4);
        x2 = textbox(j,1);
        y2 = textbox(j,2);
        w2 = textbox(j,3);
        h2 = textbox(j,4);

        graph(i,j) = x1 <= x2 && x2 <= x1 + w1 && y1 <= y2 && y2 <= y1 + h1 ...
            || x2 <= x1 && x1 <= x2 + w2 && y2 <= y1 && y1 <= y2 + h2;
        graph(j,i) = graph(i,j);
    end
end

groups = traverse(graph);
toRemove = [];
for i = 1:length(groups)
    if isscalar(groups{i})
        continue;
    end
    pos = min(textbox(groups{i},1:2));
    sz = max(textbox(groups{i},1:2) + textbox(groups{i},3:4)) - pos;
    toRemove = [toRemove groups{i}];
    textbox(end+1,:) = [pos sz];
end
textbox(toRemove,:) = [];
end



