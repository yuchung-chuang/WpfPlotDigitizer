var image = document.querySelector('img');
var clickTimes = 0;
image.onclick = function () {
  clickTimes++;
  image.src = './images/' + (clickTimes % 2 === 0 ? 'Mozilla-Firefox-icon.png' : '/Mozilla-Firefox-icon2.png');
}
var myButton = document.querySelector('button');
var myHeading = document.querySelector('h1');

function setUserName() {
  var myName = prompt('Please enter your name.');
  localStorage.setItem('name', myName);
  myHeading.innerHTML = 'Mozilla is cool, ' + myName;
}

if (!localStorage.getItem('name')) {
  setUserName();
} else {
  var storedName = localStorage.getItem('name');
  myHeading.innerHTML = 'Mozilla is cool, ' + storedName;
}

myButton.onclick = setUserName;
