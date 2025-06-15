/**
 * ImageImporter.js
 * Utility to dynamically import all images from the assets folder
 */

// Import all images
function importAll() {
  const images = {};
  for (var i = 0; i < 10; i++) {
    const female = `female${i + 1}`;
    const male = `male${i + 1}`;
    let url = `/images/${female}.jpeg`;

    images[female] = url;

    url = `/images/${male}.jpeg`;

    images[male] = url;
  }
  return images;
}

// Import all images from the assets directory
const images = importAll();

export default images;
