extension = ".jpg";
files = dir("Graph-1" + extension);
for fileID = 1:length(files)
    file = files(fileID);
    [~,filename,~] = fileparts(file.name);
    filename = string(filename);

    im = imread(filename + extension);

    gray = im2gray(im);
    binary = imbinarize(gray);
    edgeBinary = edge(binary, "roberts");

    [H,theta,rho] = hough(edgeBinary);
    peaks = houghpeaks(H,5);
    lines = houghlines(edgeBinary,theta,rho,peaks);

    % draw(im, lines)

    if length(lines) ~= 4 || ~all(abs([lines.theta]) == 0 | abs([lines.theta]) == 90)
        warning("Line detection error!")
        continue
    end

    intersections = [];
    for i = 1:length(lines)
        for j = i+1:length(lines)
            if (lines(i).theta == lines(j).theta)
                continue
            end
            matrix = @(theta1, theta2) [cosd(theta1) sind(theta1); cosd(theta2) sind(theta2)];
            intersections(end+1,:) = (matrix(lines(i).theta, lines(j).theta) \ [lines(i).rho; lines(j).rho]);
        end
    end
    if size(intersections,1) ~= 4
        warning('Intersections detection error!');
        return
    end
    
    for i = 1:size(intersections,1)
        range = -10:10;
        roi = binary(intersections(i,2) + range, intersections(i,1) + range);
        imshow(roi);
    end
    
end

function draw(im, lines)
clf
imshow(im)
hold on
max_len = 0;
for k = 1:length(lines)
    xy = [lines(k).point1; lines(k).point2];
    plot(xy(:,1),xy(:,2),'LineWidth',1,'Color','green');

    % Plot beginnings and ends of lines
    plot(xy(1,1),xy(1,2),'x','LineWidth',1,'Color','yellow');
    plot(xy(2,1),xy(2,2),'x','LineWidth',1,'Color','red');

    % Determine the endpoints of the longest line segment
    len = norm(lines(k).point1 - lines(k).point2);
    if (len > max_len)
        max_len = len;
        xy_long = xy;
    end
end
% highlight the longest line segment
plot(xy_long(:,1),xy_long(:,2),'LineWidth',1,'Color','red');
end