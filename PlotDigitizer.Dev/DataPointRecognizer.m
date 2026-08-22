extension = ".png";
files = dir("Screenshot 2024-09-15 131341" + extension);
for fileID = 1:length(files)
    file = files(fileID);
    [~,filename,~] = fileparts(file.name);
    filename = string(filename);

    im = imread(filename + extension);
    gray = rgb2gray(im);
    bw = imbinarize(gray);

    ax = [122    35   712   589];
    roi = ~imcrop(bw, ax);
    roi = imclearborder(roi);
    roi = imfill(roi, 'holes');
    
    imshow(roi);

    im2 = imopen(roi, strel('disk',3)); % remove lines
    imshow(im2)
    hold on

    props = regionprops(im2, 'Area', 'Centroid','Circularity');
    areas = [props.Area];
    areas_Z = (areas - mean(areas)) / std(areas);
    props(abs(areas_Z) > 2) = []; % remove outliers

    

    centroids = vertcat(props.Centroid);
    plot(centroids(:,1), centroids(:,2), '.');
end